/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Aggregates;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Diode;

public class DiodeContextProvider<T> : IEnumerable<DiodeContext<T>>
    where T : class, IDiodeEntity, new()
{
    private readonly IServiceProvider _serviceProvider;

    public DiodeContextProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private List<DiodeContext<T>> _contexts { get; set; } = new List<DiodeContext<T>>();

    /// <summary>
    /// Readonly list of Registered Contexts
    /// </summary>
    public IEnumerable<DiodeContext<T>> Contexts => _contexts.AsEnumerable();

    public event EventHandler<DiodeContextChangeEventArgs<T>>? StateHasChanged;

    public IEnumerable<DiodeEntityData<T>> AsEntityData
    {
        get
        {
            List<DiodeEntityData<T>> list = new();
            foreach (var item in _contexts)
                list.Add(item.AsDiodeEntityData);

            return list;
        }
    }

    /// <summary>
    /// Gets the registered context
    /// Will return a null if one doesn't exist
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public DiodeContext<T>? GetContext(Guid uid)
        => this._contexts.FirstOrDefault(s => s.Uid == uid);

    /// <summary>
    /// Tries to get the registered context
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool TryGetContext(Guid uid, [NotNullWhen(true)] out DiodeContext<T>? context)
    {
        context = this._contexts.FirstOrDefault(s => s.Uid == uid);
        return context is not null;
    }

    /// <summary>
    /// Creates a context for the provided T 
    /// and adds it to the context list  
    /// </summary>
    /// <param name="item"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public DiodeProviderResult<DiodeContext<T>> CreateorGetContext(T item, DiodeState? state = null)
    {
        var context = this._contexts.FirstOrDefault(s => s.Uid == item.Uid);

        if (context is not null)
            return DiodeProviderResult<DiodeContext<T>>.AlreadyExists(context);

        var newContext = new DiodeContext<T>(item, state ?? DiodeState.New());

        this._contexts.Add(newContext);
        this.OnStateChange(newContext.Uid, newContext);

        return DiodeProviderResult<DiodeContext<T>>.Create(newContext);
    }

    public DiodeResult<DiodeContext<T>> CreateContext(T item, DiodeState? state = null)
    {
        var context = this._contexts.FirstOrDefault(s => s.Uid == item.Uid);

        if (context is not null)
            return DiodeResult<DiodeContext<T>>.Failure($"A context already exists for {item.Uid}");

        var newContext = new DiodeContext<T>(item, state ?? DiodeState.New());

        this._contexts.Add(newContext);
        this.OnStateChange(newContext.Uid, newContext);

        return DiodeResult<DiodeContext<T>>.Success(newContext);
    }

    /// <summary>
    /// Clears the provided context from the context list
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public DataResult ClearContext(Guid uid)
    {
        var context = this._contexts.FirstOrDefault(s => s.Uid == uid);

        if (context is null)
            return DataResult.Failure($"No Store exists = {uid}");

        this._contexts.Remove(context);
        this.OnStateChange(uid, context);

        return DataResult.Success();
    }

    /// <summary>
    /// Applies the dispatched mutation action to the context
    /// </summary>
    /// <typeparam name="TAction"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<T>> DispatchAsync<TAction>(TAction action)
    where TAction : class, IDiodeAction
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
            this.OnStateChange(uid, context);
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
    public void OnStateChange(Guid uid, DiodeContext<T> sender)
        => this.StateHasChanged?.Invoke(this, new DiodeContextChangeEventArgs<T>(uid, sender));

    /// <summary>
    /// Public enumerator for the collection
    /// </summary>
    /// <returns></returns>
    public IEnumerator<DiodeContext<T>> GetEnumerator()
        => _contexts.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
}
