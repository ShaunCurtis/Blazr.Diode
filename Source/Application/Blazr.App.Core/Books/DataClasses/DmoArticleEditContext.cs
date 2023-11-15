/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class DmoArticleEditContext : IDiodeAction, IDiodeMutation<DmoArticle>
{
    public string ActionName => "Article Edit Context";

    public ArticleUid ArticleId{ get; private set; }

    public string? Title { get; set; }

    public string? Introduction { get; set; }

    public DmoArticleEditContext(DmoArticle record)
    {
        this.ArticleId = record.ArticleId;
        this.Title = record.Title;
        this.Introduction = record.Introduction;
    }

    public DmoArticleEditContext()
    {
        this.ArticleId = new(Guid.NewGuid());
        this.Title = string.Empty;
        this.Introduction = string.Empty;
    }

    public DmoArticle AsRecord => new()
    {
        ArticleId = this.ArticleId,
         Introduction = this.Introduction ?? string.Empty,
          Title = this.Title ?? string.Empty,
    };

    public DiodeAsyncMutationDelegate<DmoArticle> Mutation => (DiodeMutationRequest<DmoArticle> request) =>
    {
        var mutation = this.AsRecord;
        return Task.FromResult(DiodeResult<DmoArticle>.Success(mutation));
    };
}
