/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Core;

public readonly record struct ChapterUid(Guid Value);

public record Chapter
{
    public ChapterUid ChapterId { get; set; } = new(Guid.NewGuid());
    public BookUid BookId { get; set; } = new(Guid.NewGuid());
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}
