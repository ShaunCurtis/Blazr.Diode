/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class DmoSectionEditContext : IDiodeAction<SectionUid>, IDiodeMutation<DmoSection>
{
    public string ActionName => "Article Edit Context";

    SectionUid IDiodeAction<SectionUid>.KeyValue => this.SectionId;

    public SectionUid SectionId { get; private set; }

    public ArticleUid ArticleId { get; private set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DmoSectionEditContext(DmoSection record)
    {
        this.SectionId = record.SectionId;
        this.ArticleId = record.ArticleId;
        this.Title = record.Title;
        this.Content = record.Content;
    }

    public DmoSectionEditContext(ArticleUid articleUid)
    {
        this.SectionId = new(Guid.NewGuid());
        this.ArticleId = articleUid;
        this.Title = string.Empty;
        this.Content = string.Empty;
    }

    public DmoSection AsRecord => new()
    {
        SectionId = this.SectionId,
        ArticleId = this.ArticleId,
        Content = this.Content ?? string.Empty,
        Title = this.Title ?? string.Empty,
    };

    DiodeAsyncMutationDelegate<DmoSection> IDiodeMutation<DmoSection>.Mutation => (DiodeMutationRequest<DmoSection> request) =>
    {
        var mutation = this.AsRecord;
        return Task.FromResult(DiodeResult<DmoSection>.Success(mutation));
    };
}
