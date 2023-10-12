# State Management in Blazor and DotNetCore

The management of state within an application is a concept that many programmers come to far too late.

You can write a relatively complex application without actively considering state.  Your code will be more complex, you will create mechanisms to passive control state, and you will have hard to track mutation bugs.
 But you don't absolutely have to use it.

So why do so?

1. You learn to examine your objects more closely. You break an object down into it's state and it's logic.  That closer examination often leads to better design decisions and closer adherance to the *Single Responsibility Principle*.

2. It simplifies your application.  Initially that may seem hard to believe.  There's more complexity in implementing a state management system.  The benefits come down the road.  I recently updated an application that was pre Diode.  I was amazed at how much applkication logic and objects went into the recycle bin.

3. It makes deugging easier.  There is only place where mutations take place, making it easy to track who's mutating what and when.

## Reinventing the Wheel!

So whay do we need another DotNetCore implementation of Flux?  Fluxor is good, well tested and does what it says on the tin.

Absolutely true.  But:

1. It doesn't do store collections: you can't store a set of `WeatherForecast` objects with different entity Ids.

2. It doesn't do asynchronous operations.

## Diode

Diode is my Flux based implementation.  It isn't a true to concept implementation as it doesn't:

1. Doesn't enforce *pure method* or *idempotence* behaviour to the mutator.
2. As a mutation can be asynchronous and yield all actors are returned a shared `Task`, than completes when all the queued mutations have completed.  All queued mutations are applied sequentially and the following mutation applies it's mutation to result of the previous mutation.
3. There is no central dispatcher.  If you want one, code it.

### Data Objects

The classic `WeatherForecast` looks like this.  I've added the classic database int Id field.

```csharp
public class WeatherForecast
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}
```

We need to transform this into an immutable object.

```csharp
public record WeatherForecast
{
    public int Id { get; init; }
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; init; }
}
```

Next we need to remove the primitive obsession i.e. the Id and the Temperature.

```csharp
public readonly struct WeatherForcastId
{
    
}
```





