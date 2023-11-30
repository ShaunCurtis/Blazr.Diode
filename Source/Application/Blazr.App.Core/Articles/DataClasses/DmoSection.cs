/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct SectionUid(Guid Value);

public record DmoSection
{
    public SectionUid SectionId { get; init; } = new(Guid.NewGuid());
    public ArticleUid ArticleId { get; init; } = new(Guid.Empty);
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}
