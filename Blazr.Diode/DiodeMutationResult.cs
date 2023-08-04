/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public record DiodeMutationResult<T>
    where T : class
{
    public T? Item { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static DiodeMutationResult<T> Success(T item, string? message = null)
        => new DiodeMutationResult<T>() { Successful = true, Item = item, Message = message};

    public static DiodeMutationResult<T> Failure(string message, T? item = null)
        => new DiodeMutationResult<T>() { Successful = false, Message = message, Item = item };
}