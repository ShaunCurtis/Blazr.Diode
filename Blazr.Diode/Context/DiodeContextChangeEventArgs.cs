/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public class DiodeContextChangeEventArgs<T> : EventArgs
    where T : class, IDiodeEntity, new()
{
    public Guid Uid { get; init; }

    public DiodeContext<T> MutatedItem { get; init; }

    public DiodeContextChangeEventArgs(Guid uid, DiodeContext<T> mutatedItem)
    { 
        this.MutatedItem = mutatedItem;
        Uid = uid;
    }
    public static DiodeContextChangeEventArgs<T> Create(Guid uid, DiodeContext<T> mutatedItem)
        => new(uid, mutatedItem);
}
