/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

/// <summary>
/// Interface defining a Mutation Request
/// </summary>
public interface IDiodeAction<K>
{
    string ActionName { get; }
    public K KeyValue { get; }
}
