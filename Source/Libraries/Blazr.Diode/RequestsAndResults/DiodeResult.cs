/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public record DiodeResult<T>
    where T : class
{
    public T Item { get; init; } = default!;
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static DiodeResult<T> Success(T item, string? message = null)
        => new DiodeResult<T>() { Successful = true, Item = item, Message = message };

    public static DiodeResult<T> Failure(string message, T? item = null)
        => new DiodeResult<T>() { Successful = false, Message = message, Item = item! };
}