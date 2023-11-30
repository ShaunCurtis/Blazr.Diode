/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Diode.Composites;

public class DiodeCollectionCompositeProvider<K, T>
    where T : class, new()
{
    private readonly IServiceProvider _serviceProvider;
    private List<DiodeContextEntry<K, T>> _contexts { get; set; } = new List<DiodeContextEntry<K, T>>();

    public DiodeCollectionCompositeProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Readonly list of Registered Contexts
    /// </summary>
    public IEnumerable<DiodeContextEntry<K, T>> Contexts => _contexts.AsEnumerable();

    /// <summary>
    /// Async Callback to execute on a change
    /// </summary>
    public Func<DiodeContextChangeEventArgs<T>, ValueTask>? StateHasChanged;

    public IEnumerable<DiodeEntityData<T>> AsEntityData
    {
        get
        {
            List<DiodeEntityData<T>> list = new();
            foreach (var item in _contexts)
                list.Add(item.Context.AsDiodeEntityData);

            return list;
        }
    }

    public IEnumerable<T> AsList
    {
        get
        {
            List<T> list = new();
            foreach (var item in _contexts)
                list.Add(item.Context.ImmutableItem);

            return list;
        }
    }

    /// <summary>
    /// Gets the registered context
    /// Will return a null if one doesn't exist
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public DiodeContext<T>? GetContext(K key)
    {
        if (key == null)
            return null;

        return this._contexts.FirstOrDefault(s => key.Equals(s.Key))?.Context;
    }

    /// <summary>
    /// Tries to get the registered context
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool TryGetContext(K key, [NotNullWhen(true)] out DiodeContext<T>? context)
    {
        context = null;
        if (key == null)
            return false;

        context = this._contexts.FirstOrDefault(s => key.Equals(s.Key))?.Context;

        return context is not null;
    }

    /// <summary>
    /// Adds Context to the existing collection
    /// </summary>
    /// <param name="key">Id key</param>
    /// <param name="item">The Item</param>
    /// <param name="state">optional State</param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<DiodeContext<T>>> AddContextAsync(K key, T item, DiodeState? state = null)
    {
        DiodeContext<T>? context = null;

        if (key != null)
            context = this._contexts.FirstOrDefault(s => key.Equals(s.Key))?.Context;

        if (context is not null)
            return DiodeResult<DiodeContext<T>>.Failure($"A context already exists for key : {key?.ToString()}");

        var newContext = new DiodeContext<T>(item, state ?? DiodeState.New());

        this._contexts.Add(new(key, newContext));
        await this.OnStateChangeAsync(key, newContext);

        return DiodeResult<DiodeContext<T>>.Success(newContext);
    }

    /// <summary>
    /// Applies the dispatched mutation action to the context
    /// </summary>
    /// <typeparam name="TAction"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<T>> DispatchAsync<TAction>(TAction action)
    where TAction : class, IDiodeAction<K>
    {
        var context = this.GetContext(action.KeyValue);

        // If the store is null then we need to create a new one
        if (context is null)
        {
            await this.AddContextAsync(action.KeyValue, new T(), DiodeState.New());
            context = this.GetContext(action.KeyValue);
        }

        // If the store is still null we have a problem
        if (context is null)
            return DiodeResult<T>.Failure($"Could not locate a registered context for {typeof(T).Name} and Key : {action.KeyValue?.ToString()}");

        MutationResult<T> result;

        // check if TAction is a self contained Mutation Action
        if (action is IDiodeMutation<T> mutationAction)
        {
            // Queues the Mutation's DiodeMutationDelegate onto the store's mutation queue
            result = await context.QueueAsync(mutationAction.Mutation);

            if (result.IsMutated)
                await this.OnStateChangeAsync(action.KeyValue, context);

            return DiodeResult<T>.Success(result.Item);
        }

        // We have an action that requires a handler
        // Gets the DI registered action from the DI Provider
        var handler = _serviceProvider.GetService<IDiodeHandler<K, T, TAction>>();

        // deal with a null Handler
        if (handler is null)
            return DiodeResult<T>.Failure($"Could not locate a registered handler for {typeof(TAction).Name}");

        handler.Action = action;

        // Queues the DiodeMutationDelegate onto the context's mutation queue
        result = await context.QueueAsync(handler.Mutation);

        if (result.IsMutated)
            await this.OnStateChangeAsync(handler.Action.KeyValue, context);

        return DiodeResult<T>.Success(result.Item);
    }

    /// <summary>
    /// Applies the provided mutation delegate to the context 
    /// </summary>
    /// <param name="mutationDelegate"></param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<T>> DispatchDelegateAsync(DiodeAsyncMutationDelegate<T> mutationDelegate, K key)
    {
        var context = this.GetContext(key);

        // If the store is null then we need to create a new one
        if (context is null)
        {
            await this.AddContextAsync(key, new T(), DiodeState.New());
            context = this.GetContext(key);
        }

        // If the store is still null then we have a problem.
        if (context is null)
            return DiodeResult<T>.Failure($"No context exists for {typeof(T).Name} and Key : {key?.ToString()}");

        var result = await context.QueueAsync(mutationDelegate);

        if (result.IsMutated)
            await this.OnStateChangeAsync(key, context);

        return DiodeResult<T>.Success(result.Item);
    }

    /// <summary>
    /// Resets the state on the context
    /// </summary>
    /// <param name="uid"></param>
    public async ValueTask<DataResult> MarkContextAsPersistedAsync(K key)
    {
        if (this.TryGetContext(key, out DiodeContext<T>? context))
        {
            context.MarkAsPersisted();
            await this.OnStateChangeAsync(key, context);
            return DataResult.Success();
        }

        return DataResult.Failure($"No Context exists for Key:  {key?.ToString()}");
    }

    /// <summary>
    /// Sets the deletion flag on the context
    /// T record will be deleted from the data store when the context is next persisted
    /// </summary>
    /// <param name="uid"></param>
    public async ValueTask<DataResult> MarkContextForDeletionAsync(K key)
    {
        if (this.TryGetContext(key, out DiodeContext<T>? context))
        {
            context.MarkForDeletion();
            await this.OnStateChangeAsync(key, context);
            return DataResult.Success();
        }

        return DataResult.Failure($"No Context exists for Key: {key?.ToString()}");
    }

    private async ValueTask OnStateChangeAsync(object? key, DiodeContext<T> sender)
    {
        if (this.StateHasChanged is not null)
            await this.StateHasChanged.Invoke(new DiodeContextChangeEventArgs<T>(key, sender));
    }
}
