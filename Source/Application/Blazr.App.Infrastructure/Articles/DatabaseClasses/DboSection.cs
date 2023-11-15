/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed record DboSection : ICommandEntity, IKeyedEntity
{
    [Key] public Guid SectionId { get; init; } = Guid.NewGuid();
    public Guid ArticleId { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;

    public object KeyValue => SectionId;
}
