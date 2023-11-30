/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Server.Web.Counter;

public record CounterIncrementAction : IDiodeAction<Guid>
{
    public Guid KeyValue { get; init; }
    public string ActionName => $"Increment Counter";

    public CounterIncrementAction(Guid uid)
    {
        this.KeyValue = uid;
    }
}
