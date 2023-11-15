/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed record DboArticle : ICommandEntity, IKeyedEntity
{
    [Key] public Guid ArticleId { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public string Introduction { get; init; } = string.Empty;
    public List<Section> Sections { get; init; } = new List<Section>();

    public object KeyValue => ArticleId;
}
