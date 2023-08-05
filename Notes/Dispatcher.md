# Dispatcher

The Dispatcher is the broker for the system.  It finds the store for the `T` type and resolves the correct handler from the DI service provider.  THere's only one instance of the Dispatcher.

The dispatcher provides two `DispatchAsync` implementations.

The first takes a action, revolves a handler and store for the action, and queues the handler's `Mutation` delegate onto the store.

```csharp
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

    var mutatedT = await store.QueueAsync(handler.Mutation);

    return DiodeMutationResult<T>.Success(mutatedT);
}
```

The second takes a `DiodeMutationDelegate` and queues the handler's `Mutation` delegate onto the store.

```csharp
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
```