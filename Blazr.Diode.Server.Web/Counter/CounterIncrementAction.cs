namespace Blazr.Diode.Server.Web.Counter;

public record CounterIncrementAction : IDiodeAction
{
    public string Name => "Increment Counter by x";

    public int IncrementBy { get; init; } = 1;    
}
