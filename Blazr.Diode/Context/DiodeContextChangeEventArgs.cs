/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public class DiodeContextChangeEventArgs<T> : EventArgs
    where T : class, new()
{
    public object? KeyValue { get; init; }

    public DiodeContext<T> MutatedItem { get; init; }

    public DiodeContextChangeEventArgs(object? key,  DiodeContext<T> mutatedItem)
    { 
        this.KeyValue = key;
        this.MutatedItem = mutatedItem;
    }
    public static DiodeContextChangeEventArgs<T> Create(object key, DiodeContext<T> mutatedItem)
        => new(key, mutatedItem);
}
