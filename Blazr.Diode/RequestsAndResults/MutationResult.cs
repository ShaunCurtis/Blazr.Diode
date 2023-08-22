/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public record MutationResult<T>
    where T : class
{
    public T Item { get; init; }
    public bool IsMutated { get; init; }
    public string? Message { get; init; }

    public MutationResult(T item, bool isMutated = true)
    {
        this.Item = item;
        this.IsMutated = isMutated;
    }

    public static MutationResult<T> Mutated(T item, string? message = null)
        => new MutationResult<T>(item, true) { Message = message };

    public static MutationResult<T> NotMutated(T item, string? message = null)
        => new MutationResult<T>(item, false) { Message = message };
}