/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Diode;

public record DiodeContextEntry<T, K>(K Key, DiodeContext<T> Context)
    where T : class, IDiodeEntity, new();

public class DiodeContextProvider<T, K>
    where T : class, IDiodeEntity, new()
{
    private readonly IServiceProvider _serviceProvider;

    public DiodeContextProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private List<DiodeContextEntry<T, K>> _contexts { get; set; } = new List<DiodeContextEntry<T, K>>();

    /// <summary>
    /// Readonly list of Registered Contexts
    /// </summary>
    public IEnumerable<DiodeContextEntry<T, K>> Contexts => _contexts.AsEnumerable();

    public event EventHandler<DiodeContextChangeEventArgs<T>>? StateHasChanged;

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
    /// Creates a context for the provided T 
    /// and adds it to the context list  
    /// </summary>
    /// <param name="item"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public DiodeProviderResult<DiodeContext<T>> CreateorGetContext(K key, T item, DiodeState? state = null)
    {
        DiodeContext<T>? context = null;

        if (key != null)
            context = this._contexts.FirstOrDefault(s => key.Equals(s.Key))?.Context;

        if (context is not null)
            return DiodeProviderResult<DiodeContext<T>>.AlreadyExists(context);

        var newContext = new DiodeContext<T>(item, state ?? DiodeState.New());

        this._contexts.Add(new(key,newContext));
        this.OnStateChange(newContext);

        return DiodeProviderResult<DiodeContext<T>>.Create(newContext);
    }

    public DiodeResult<DiodeContext<T>> CreateContext(K key, T item, DiodeState? state = null)
    {

        DiodeContext<T>? context = null;

        if (key != null)
            context = this._contexts.FirstOrDefault(s => key.Equals(s.Key))?.Context;

        if (context is not null)
            return DiodeResult<DiodeContext<T>>.Failure($"A context already exists for {item.Uid}");

        var newContext = new DiodeContext<T>(item, state ?? DiodeState.New());

        this._contexts.Add(new(key, newContext));
        this.OnStateChange(newContext);

        return DiodeResult<DiodeContext<T>>.Success(newContext);
    }

    /// <summary>
    /// Clears the provided context from the context list
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public DataResult ClearContext(K key)
    {
        DiodeContext<T>? context = null;

        if (key != null)
            context = this._contexts.FirstOrDefault(s => key.Equals(s.Key))?.Context;

        if (context is null)
            return DataResult.Failure($"No Store exists = {key?.ToString()}");

        this._contexts.Remove(new(key,context));
        this.OnStateChange(context);

        return DataResult.Success();
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
        var context = this.GetContext(action.Uid);

        // deal with a null store
        if (context is null)
            return DiodeResult<T>.Failure($"Could not locate a registered context for {typeof(T).Name} and ID : {action.Uid}");

        MutationResult<T> result;

        // check if TAction is a self contained Mutation Action
        if (action is IDiodeMutation<T> mutationAction)
        {
            // Queues the Mutation's DiodeMutationDelegate onto the store's mutation queue
            result = await context.QueueAsync(mutationAction.Mutation);

            if (result.IsMutated)
                this.OnStateChange(result.Item.Uid, context);

            return DiodeResult<T>.Success(result.Item);
        }

        // We have an action that requires a handler
        // Gets the DI registered action from the DI Provider
        var handler = _serviceProvider.GetService<IDiodeHandler<T, TAction>>();

        // deal with a null Handler
        if (handler is null)
            return DiodeResult<T>.Failure($"Could not locate a registered handler for {typeof(TAction).Name}");

        handler.Action = action;

        // Queues the DiodeMutationDelegate onto the context's mutation queue
        result = await context.QueueAsync(handler.Mutation);

        if (result.IsMutated)
            this.OnStateChange(result.Item.Uid, context);

        return DiodeResult<T>.Success(result.Item);
    }

    /// <summary>
    /// Applies the provided mutation delegate to the context 
    /// </summary>
    /// <param name="mutationDelegate"></param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<T>> DispatchDelegateAsync(DiodeAsyncMutationDelegate<T> mutationDelegate, Guid uid)
    {
        var context = this.GetContext(uid);

        // deal with a null store
        if (context is null)
            return DiodeResult<T>.Failure($"Could not locate a registered context for {typeof(T).Name} and ID : {uid}");

        var result = await context.QueueAsync(mutationDelegate);

        if (result.IsMutated)
            this.OnStateChange(result.Item.Uid, context);

        return DiodeResult<T>.Success(result.Item);
    }

    /// <summary>
    /// Resets the state on the context
    /// </summary>
    /// <param name="uid"></param>
    public DataResult MarkContextAsPersisted(Guid uid)
    {
        if (this.TryGetContext(uid, out DiodeContext<T>? context))
        {
            context.MarkAsPersisted();
            this.OnStateChange(uid, context);
            return DataResult.Success();
        }

        return DataResult.Failure($"No Context exists for ID: {uid}");
    }

    /// <summary>
    /// Sets the deletion flag on the context
    /// T record will be deleted from the data store when the context is next persisted
    /// </summary>
    /// <param name="uid"></param>
    public DataResult MarkContextForDeletion(Guid uid)
    {
        if (this.TryGetContext(uid, out DiodeContext<T>? context))
        {
            context.MarkForDeletion();
            this.OnStateChange(context);
            return DataResult.Success();
        }

        return DataResult.Failure($"No Context exists for ID: {uid}");
    }

    /// <summary>
    /// Method called through the IDiodeProvider interface
    /// Contexts call this method to inform the provider of a mutation
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="sender"></param>
    public void OnStateChange(DiodeContext<T> sender)
        => this.StateHasChanged?.Invoke(this, new DiodeContextChangeEventArgs<T>(sender));
}
