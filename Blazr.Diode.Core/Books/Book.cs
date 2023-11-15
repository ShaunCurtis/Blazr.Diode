/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Core;

public readonly record struct BookUid(Guid Value);
public record Book
{
    public BookUid BookId { get; init; } = new(Guid.NewGuid());
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<Chapter> Chapters { get; init; } = new List<Chapter>();  
}