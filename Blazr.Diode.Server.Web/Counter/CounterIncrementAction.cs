namespace Blazr.Diode.Server.Web.Counter;

public record CounterIncrementAction : IDiodeAction
{
    public string Name => $"Increment by {this.IncrementBy}";

    public int IncrementBy { get; init; } = 1;    
}
