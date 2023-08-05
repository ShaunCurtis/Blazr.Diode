# Diode Actions

The purpose of an action is to provide the data for a mutation.  Actions are defined as `records`.

`Actions` implement the `IDiodeAction` interface.

```csharp
public interface IDiodeAction
{
    string Name { get; }
}
```

The name is used to provide a user friendly name for logging and display.

`CounterIncrementAction` is a typical action.

```csharp
public record CounterIncrementAction : IDiodeAction
{
    public string Name => $"Increment by {this.IncrementBy}";
    public int IncrementBy { get; init; } = 1;    
}
```

In the demo application you can see the `Name` property used for the button text.
