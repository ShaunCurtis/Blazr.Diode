# Diode Handlers

Handlers define how the changes defined in an action are applied to the store.

All handlers implement the `IDiodeHandler` interface.
 - `T` is the store type.  In the demo `CounterData`.
 - `TAction` is the action that defines the mutation data.

```csharp
public interface IDiodeHandler<T, TAction>
    where T : class
    where TAction : class, IDiodeAction
{
    public TAction Action { get; set; }
    public DiodeMutationDelegate<T> Mutation { get; }
}
```

Handlers are defined as `Transient` services.

`CounterIncrementHandler` is a typical handler.

```csharp
public class CounterIncrementHandler : IDiodeHandler<CounterData, CounterIncrementAction>
{
    public CounterIncrementAction Action { get; set; } = default!;
    public DiodeMutationDelegate<CounterData> Mutation => MutateAsync;

    public Task<DiodeMutationResult<CounterData>> MutateAsync(DiodeMutationRequest<CounterData> request)
    {
        ArgumentNullException.ThrowIfNull(Action);

        var result = request.Item with { Counter = request.Item.Counter + Action.IncrementBy };
        return Task.FromResult(DiodeMutationResult<CounterData>.Success(result));
    }
}
```

The dispatcher demonstrates how it's used.


```csharp
    public async ValueTask<DiodeMutationResult<T>> DispatchAsync<T, TAction>(TAction action)
        where T : class, new()
        where TAction : class, IDiodeAction
    {
        // Gets the DI registered action from the DI Provider
        var handler = _serviceProvider.GetService<IDiodeHandler<T, TAction>>();
        // Null checking

        // Assign the action instance
        handler.Action = action;

        // Gets the DI registered store from the DI Provider
        var store = _serviceProvider.GetService<DiodeStore<T>>();
        // Null checking

        // Queues the DiodeMutationDelegate onto the store's mutation queue
        var mutatedT = await store.QueueAsync(handler.Mutation);

        // return the mutated value
        return DiodeMutationResult<T>.Success(mutatedT);
    }
```

