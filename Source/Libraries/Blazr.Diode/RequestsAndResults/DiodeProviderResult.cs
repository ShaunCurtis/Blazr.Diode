/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public record DiodeProviderResult<T>
    where T : class
{
    public T Item { get; init; }
    public bool Exists { get; init; }

    private DiodeProviderResult(T item, bool exists)
    {
        this.Item = item;
        this.Exists = exists;
    }

    public static DiodeProviderResult<T> AlreadyExists(T item)
        => new DiodeProviderResult<T>(item, true);

    public static DiodeProviderResult<T> Create(T item)
        => new DiodeProviderResult<T>(item, false);
}