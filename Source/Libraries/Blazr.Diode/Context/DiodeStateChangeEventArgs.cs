/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public class DiodeStateChangeEventArgs : EventArgs
{
    public Guid Uid { get; init; }

    public object? MutatedItem { get; init; }

    public DiodeStateChangeEventArgs(Guid uid, object? mutatedItem)
    { 
        this.MutatedItem = mutatedItem;
        Uid = uid;
    }
    public static DiodeStateChangeEventArgs Create(Guid uid, object? mutatedItem)
        => new(uid, mutatedItem);
}
