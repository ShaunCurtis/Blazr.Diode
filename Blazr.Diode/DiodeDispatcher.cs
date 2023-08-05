/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public class DiodeDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DiodeDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Dispatches the action to the correct handler to effect the mutation
    /// </summary>
    /// <typeparam name="TProcess"></typeparam>
    /// <param name="process"></param>
    /// <returns></returns>
    public async ValueTask<DiodeMutationResult<T>> DispatchAsync<T, TAction>(TAction action)
        where T : class, new()
        where TAction : class, IDiodeAction
    {
        // Gets the DI registered action from the DI Provider
        var handler = _serviceProvider.GetService<IDiodeHandler<T, TAction>>();

        // deal with a null Handler
        if (handler is null)
            return DiodeMutationResult<T>.Failure($"Could not locate a registered handler for {typeof(TAction).Name}");

        handler.Action = action;

        // Gets the DI registered store from the DI Provider
        var store = _serviceProvider.GetService<DiodeStore<T>>();

        // deal with a null store
        if (store is null)
            return DiodeMutationResult<T>.Failure($"Could not locate a registered store for {typeof(T).Name}");

        // Queues the DiodeMutationDelegate onto the store's mutation queue
        var mutatedT = await store.QueueAsync(handler.Mutation);

        return DiodeMutationResult<T>.Success(mutatedT);
    }

    /// <summary>
    /// Dispatches the OwsMutationDelegate to effect the mutation
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    /// <exception cref="StateDoesNotExistsException"></exception>
    public async ValueTask<DiodeMutationResult<T>> DispatchDelegateAsync<T>(DiodeMutationDelegate<T> mutationDelegate)
        where T : class, new()
    {
        // Gets the DI registered store from the DI Provider
        var store = _serviceProvider.GetService<DiodeStore<T>>();

        // deal with a null store
        if (store is null)
            return DiodeMutationResult<T>.Failure($"Could not locate a registered store for {typeof(T).Name}");

        var mutatedT = await store.QueueAsync(mutationDelegate);

        // Queues the DiodeMutationDelegate onto the store's mutation queue
        return DiodeMutationResult<T>.Success(mutatedT);
    }
}
