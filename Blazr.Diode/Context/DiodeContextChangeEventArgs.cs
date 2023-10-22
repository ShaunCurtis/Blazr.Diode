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

    public DiodeContextChangeEventArgs(DiodeContext<T> mutatedItem)
    { 
        this.MutatedItem = mutatedItem;
    }
    public static DiodeContextChangeEventArgs<T> Create(DiodeContext<T> mutatedItem)
        => new(mutatedItem);
}
