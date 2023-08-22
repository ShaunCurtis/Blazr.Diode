/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Entities;

public class DiodeEntityDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DiodeEntityDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    /// <summary>
    /// Dispatches the action to the correct handler to effect the mutation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TAction"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<T>> DispatchAsync<T, TAction>(TAction action)
        where T : class, IDiodeEntity, new()
        where TAction : class, IDiodeAction, IDiodeEntityAction
    {
        // Gets the DI registered action from the DI Provider
        var handler = _serviceProvider.GetService<IDiodeHandler<T, TAction>>();

        // deal with a null Handler
        if (handler is null)
            return DiodeResult<T>.Failure($"Could not locate a registered handler for {typeof(TAction).Name}");

        handler.Action = action;

        // Gets the DI registered store from the DI Provider
        var storeProvider = _serviceProvider.GetService<DiodeStoreProvider<T>>();

        // deal with a null store Provider
        if (storeProvider is null)
            return DiodeResult<T>.Failure($"Could not locate a registered store Provider for {typeof(T).Name}");

        var store = storeProvider.GetStore(action.Uid);

        // deal with a null store
        if (store is null)
            return DiodeResult<T>.Failure($"Could not locate a registered store for {typeof(T).Name} and ID : {action.Uid}");

        // Queues the DiodeMutationDelegate onto the store's mutation queue
        var mutatedT = await store.QueueAsync(handler.Mutation);

        return DiodeResult<T>.Success(mutatedT);
    }

    /// <summary>
    /// Dispatches the MutationDelegate to effect the mutation
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    /// <exception cref="StateDoesNotExistsException"></exception>
    public async ValueTask<DiodeResult<T>> DispatchDelegateAsync<T>(DiodeMutationDelegate<T> mutationDelegate, DiodeUid uid)
        where T : class, IDiodeEntity, new()
    {
        // Gets the DI registered store from the DI Provider
        var storeProvider = _serviceProvider.GetService<DiodeStoreProvider<T>>();

        // deal with a null store Provider
        if (storeProvider is null)
            return DiodeResult<T>.Failure($"Could not locate a registered store Provider for {typeof(T).Name}");

        var store = storeProvider.GetStore(uid);

        // deal with a null store
        if (store is null)
            return DiodeResult<T>.Failure($"Could not locate a registered store for {typeof(T).Name} and ID : {uid}");

        var mutatedT = await store.QueueAsync(mutationDelegate);

        // Queues the DiodeMutationDelegate onto the store's mutation queue
        return DiodeResult<T>.Success(mutatedT);
    }
}
