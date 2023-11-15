/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class ArticleMap : IDboEntityMap<DboArticle, DmoArticle>
{
    public DmoArticle MapTo(DboArticle item)
        => Map(item);

    public DboArticle MapTo(DmoArticle item)
        => Map(item);

    public static DmoArticle Map(DboArticle item)
        => new()
        {
            ArticleId = new(item.ArticleId),
            Introduction = item.Introduction,
            Title = item.Title,
        };

    public static DboArticle Map(DmoArticle item)
        => new()
        {
             ArticleId = item.ArticleId.Value,
             Introduction = item.Introduction,
             Title = item.Title,
        };
}
