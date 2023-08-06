# Blazr.Diode

Diode is an asynchronous state management system based on the **Flux** pattern.


![Diode Data Flow](./images/diode-dataflow.png)


Players are front end services such presenters or views, or backend services interfaces to external services.  They create `Action` instances populated with the relevant mutation data and *Dispatch* the action to the `Dispatcher`.

The dispatcher's job is to resolve the correct handler and queue for the action and dispatch it on.  In this implementation it resolves the registered handle service from the DI container, sets the action instance on the handler, and queues the handler's `Mutate` property delegate on the relevant store's mutation queue.

The store executes the delegate, returns a `DiodeMutationResult` and raises the `StateHasChanged` to notify any listeners.


The accompanying project demonstrates a simple  `Counter` page implementation.

## CounterData

`CounterData` is the state object.

```csharp
public record CounterData
{
    public int Counter { get; init; }
}
```

## CounterIncrementAction

`CounterIncrementAction` defines the data we need to effect the increment mutation.  In this case it just defines the amount to increment the value by.  It must implement `IDiodeAction`.  

```csharp
public record CounterIncrementAction : IDiodeAction
{
    public string Name => "Increment Counter by x";

    public int IncrementBy { get; init; } = 1;    
}
```

## CounterIncrementHandler

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

Handlers are Transient based services.

`CounterIncrementHandler` implements `IDiodeHandler`.

`MutateAsync` applies the mutation.  It takes data provided in the action and creates a new `CounterData` instance.

```csharp
public class CounterIncrementHandler : IDiodeHandler<CounterData, CounterIncrementAction>
{
    public CounterIncrementAction Action { get; set; } = default!;

    public Task<DiodeMutationResult<CounterData>> MutateAsync(DiodeMutationRequest<CounterData> request)
    {
        ArgumentNullException.ThrowIfNull(Action);

        var result = request.Item with { Counter = request.Item.Counter + Action.IncrementBy };
        return Task.FromResult(DiodeMutationResult<CounterData>.Success(result));
    }
}
```

## Services

The services are declared in a `IServiceCollection` extension method.

```csharp
public static class CounterServices
{
    public static void AddCounterServices(this IServiceCollection services)
    {
        services.AddScoped<DiodeStore<CounterData>>();
        services.AddScoped<DiodeDispatcher>();
        services.AddTransient<IDiodeHandler<CounterData,CounterIncrementAction>, CounterIncrementHandler>();
    }
}
```

And added to `Program`:

```csharp
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddCounterServices();
```

## Counter

![Counter](./images/counter.png)

```csharp
@page "/counter"
@using Blazr.Diode.Server.Web.Counter;

@inject DiodeStore<CounterData> Store
@inject DiodeDispatcher Dispatcher

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @Store.Item.Counter</p>
<div class="row">

    <div class="col-12 col-sm-6 col-md-4 mb-3">
        <button class="btn btn-primary" @onclick="IncrementCount">@_action.Name</button>
    </div>

    <div class="col-12 col-sm-6 col-md-8 mb-3">
        <input class="form-range" type="range" min="1" max="10" @bind=_range @bind:after="this.RangeChanged" />
    </div>

</div>

@code {
    private CounterIncrementAction _action = new CounterIncrementAction() { IncrementBy = 1 };
    private int _range = 1;

    private async Task IncrementCount()
    {
        await Dispatcher.DispatchAsync<CounterData, CounterIncrementAction>(_action);
    }

    private void RangeChanged()
    {
        _action = new CounterIncrementAction() { IncrementBy = _range };
    }
}
```