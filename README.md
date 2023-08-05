# Blazr.Diode

Diode is a simple async unidirectional state management implmentation based on the **Flux** pattern.

The accompanying project demonstrates a simple implementation on the `Counter` page.

## CounterData

`CounterData` is the state object.

```csharp
public record CounterData
{
    public int Counter { get; init; }
}
```

`CounterIncrementAction` defines the data we need to effect the increment mutation.  In this case it just defines the amount to increment the value by.  It must implement `IDiodeAction`.  

```csharp
public record CounterIncrementAction : IDiodeAction
{
    public string Name => "Increment Counter by x";

    public int IncrementBy { get; init; } = 1;    
}
```

Each Action requires a handler that implements `IDiodeHandler`.

```csharp
public class CounterIncrementHandler : IDiodeHandler<CounterData, CounterIncrementAction>
{
    public CounterIncrementAction Action { get; set; } = default!;

    public Task<DiodeMutationResult<CounterData>> MutateAsync(DiodeMutationRequest<CounterData> request)
    {
        ArgumentNullException.ThrowIfNull(Action);

        var result = request.Item with { Counter = request.Item.Counter + Action.IncrementBy };
        return Task.FromResult( DiodeMutationResult<CounterData>.Success(result));
    }
}
```

Handlers are instantiated in the context of the DI container.  You can define services in the constructor which will populated by the Dispatcher. 

