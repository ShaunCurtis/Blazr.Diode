/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public class DiodeStateChangeEventArgs : EventArgs
{
    public object? MutatedItem { get; init; }

    public DiodeStateChangeEventArgs(object? mutatedItem)
        => this.MutatedItem = mutatedItem;

    public static DiodeStateChangeEventArgs Create(object? mutatedItem)
        => new(mutatedItem);
}
