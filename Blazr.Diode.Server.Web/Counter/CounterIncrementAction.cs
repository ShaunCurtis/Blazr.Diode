/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Server.Web.Counter;

public record CounterIncrementAction : IDiodeAction
{
    public Guid Uid { get; init; }
    public string ActionName => $"Increment Counter";

    public CounterIncrementAction(Guid uid)
    {
        Uid = uid;
    }
}
