/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct ArticleUid(Guid Value);
public record DmoArticle
{
    public ArticleUid ArticleId { get; init; } = new(Guid.NewGuid());
    public string Title { get; init; } = string.Empty;
    public string Introduction { get; init; } = string.Empty;
}
