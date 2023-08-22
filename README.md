# Blazr.Diode

Diode is an asynchronous state management system based on the **Flux** pattern.

It tracks multiple copies of the same object type in a `DiodeContext` identified by a unique identifier.

`DiodeContext` instances are accessed through a DI registered `DiodeContextProvider`.  Actions are defined as `IDiodeAction`s and dispatched through the provider.

## The Counter Example

The out-of-the-box `Counter` page provides a simple example to demonstrate the basics.

Counter code resides in the `Counter` directory including `Counter.razor`.

### State Object

`CounterData` is the state object to track.  It:

1. Is an immutable value object.
2. Implements `IDiodeEntity`.
3. Has a unique Uid.

```csharp
public record CounterData : IDiodeEntity
{
    public int Counter { get; init; }
    public int Incrementer { get; init; } = 1;
    public Guid Uid { get; init; }  = Guid.NewGuid();
}
```

### The Actions

First `CounterSetIncrementerAction` which changes the incrementer.  We'll wire this up to a range input in the control so we can change the value dynamically.

1. It's immutable.  
2. It implements `IDiodeAction` 
3. It implements `IDiodeMutation<CounterData>` and defines it's own mutator.  There's no handler. 
4. `Uid` is the `Uid` of the `DiodeContext` that the mutation will be applied to.

```csharp
public record CounterSetIncrementerAction : IDiodeAction, IDiodeMutation<CounterData>
{
    public Guid Uid { get; init; }
    public string ActionName => $"Set Incrementer";

    public int Incrementer { get; init; } = 1;

    public CounterSetIncrementerAction(Guid uid)
    {
        Uid = uid;
    }

    public DiodeAsyncMutationDelegate<CounterData> Mutation => (DiodeMutationRequest<CounterData> request) =>
    {
        var mutation = request.Item with { Incrementer = this.Incrementer };

        return Task.FromResult(DiodeResult<CounterData>.Success(mutation));
    };
}
```

Second `CounterIncrementAction` which does the actual incrementing.  It demostrates an action with a separate handler.

```csharp
public record CounterIncrementAction : IDiodeAction
{
    public Guid Uid { get; init; }
    public string ActionName => $"Increment Counter";

    public CounterIncrementAction(Guid uid)
    {
        Uid = uid;
    }
}
```

And it's `CounterIncrementHandler` mutation handler:

```csharp
public class CounterIncrementHandler : IDiodeHandler<CounterData, CounterIncrementAction>
{
    public CounterIncrementAction? Action { get; set; } = default;

    public DiodeAsyncMutationDelegate<CounterData> Mutation => (DiodeMutationRequest<CounterData> request) =>
    {
        var mutation = request.Item with { Counter = request.Item.Counter + request.Item.Incrementer };

        return Task.FromResult(DiodeResult<CounterData>.Success(mutation));
    };
}
```

The motation is defined as a `DiodeAsyncMutationDelegate`.  In both of these actions that's set to an anonymous method which receives the old `T`` and outputs a new `T`` with the mutation applied.  `T` is wrapped in request and the result value objects to provide status information. 

### Counter

`Counter` has changed significantly.  It's no longer a page, just a component.

```csharp
@implements IDisposable
@inject DiodeContextProvider<CounterData> DiodeProvider;
<PageTitle>Counter</PageTitle>

<div class="bg-light mt-3 p-2 border border-1">

    <h1>Counter</h1>

    <p role="status">Current count:@_context.ImmutableItem.Counter</p>

    <div class="row">

        <div class="col-12 col-sm-6 col-md-4 mb-3">
            <button class="btn btn-primary" @onclick="IncrementCount">@_buttonText</button>
        </div>

        <div class="col-12 col-sm-6 col-md-8 mb-3">
            <input class="form-range" type="range" min="1" max="10" @bind=_range @bind:after="this.RangeChanged" />
        </div>

    </div>

    <div class="mt-2">Diode Context Uid : @this.DataUid</div>
</div>

@code {
    [Parameter] public Guid DataUid { get; set; } = _counterUid;

    private static Guid _counterUid = Guid.NewGuid();
    private int _range = 1;
    private CounterIncrementAction _action = new(_counterUid);
    private CounterSetIncrementerAction _setIncrementAction = new(_counterUid);
    private DiodeContext<CounterData> _context = default!;
    private bool _mutating;
    private string _buttonText => $"Increment by {_context.ImmutableItem.Incrementer}";


    protected override void OnInitialized()
    {
        var result = this.DiodeProvider.CreateorGetContext(new() { Uid = this.DataUid });
        _context = result.Item;
        _range = _context.ImmutableItem.Incrementer;
        _action = _action with { Uid = this.DataUid };
        _setIncrementAction = _setIncrementAction with { Uid = this.DataUid, Incrementer = _range };
        _context.StateHasChanged += CounterStateChanged;
    }

    private async Task IncrementCount()
    {
        _mutating = true;
        await this.DiodeProvider.DispatchAsync(_action);
        _mutating = false;
    }

    private async Task RangeChanged()
    {
        _setIncrementAction = _setIncrementAction with { Incrementer = _range };

        _mutating = true;
        await this.DiodeProvider.DispatchAsync(_setIncrementAction);
        _mutating = false;
    }

    public void CounterStateChanged(object? sender, DiodeContextChangeEventArgs<CounterData> e)
    {
        if (_mutating)
            return;

        _range = _context.ImmutableItem.Incrementer;
        this.StateHasChanged();
    }

    public void Dispose()
    {
        _context.StateHasChanged -= CounterStateChanged;
    }
}
```

1. There's a default static Guid for this component, which all instances will use if one isn't set in  `DataUid`.
2. The actions are created with default values and then set up with current values from the context in `OnInitialized`.
3. The context will never be null when the component renders as it's set in `OnInitialized`, so we *null forgive* in it's declaration.
4. The component hooks up a hanlder to monitor change events on the context.  It uses `_mutating` to prevent double rendering when it's the source of the mutation, and a automatic render event will happen when the Ui event that triggered it completes.
 

### CounterPage

`CounterPage` is the new page which defines several counters.

```csharp
@page "/counter"

<h3>CounterPage</h3>

<Counter />

<Counter DataUid="Guid.Empty" />

<Counter DataUid="Guid.Empty" />
```

### Services

Finally we need to add the Diode Context Provider service for `CounterData` to DI:

```csharp
builder.Services.AddScoped<DiodeContextProvider<CounterData>>();
```

![application](./images/counter.png)