/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class SectionMap : IDboEntityMap<DboSection, DmoSection>
{
    public DmoSection MapTo(DboSection item)
        => Map(item);

    public DboSection MapTo(DmoSection item)
        => Map(item);

    public static DmoSection Map(DboSection item)
        => new()
        {
            SectionId = new(item.SectionId),
            ArticleId = new(item.ArticleId),
            Title = item.Title,
            Content = item.Content,
        };

    public static DboSection Map(DmoSection item)
        => new()
        {
            SectionId = item.SectionId.Value,
            ArticleId = item.ArticleId.Value,
            Title = item.Title,
            Content = item.Content,
        };
}
