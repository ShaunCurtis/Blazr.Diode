/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Diagnostics.CodeAnalysis;

namespace Blazr.App.Core;

public class ArticleComposite
{
    private IServiceProvider _serviceProvider;
    private DiodeSingletonCompositeProvider<DmoArticle> _root;
    private DiodeCollectionCompositeProvider<SectionUid, DmoSection> _sections;

    public DmoArticle Article => _root.Context.ImmutableItem;
    public IEnumerable<DmoSection> Sections => _sections.AsList.AsEnumerable();

    public DiodeEntityData<DmoArticle> ArticleEntityData => _root.AsEntityData;
    public IEnumerable<DiodeEntityData<DmoSection>> SectionEntityData => _sections.AsEntityData;

    public event EventHandler? StateHasChanged;

    public ArticleComposite(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _root = new(_serviceProvider);
        _sections = new(_serviceProvider);

        // These don't need unwiring as the objects are privately owned by this 
        _root.StateHasChanged = this.OnArticleStateChanged;
        _sections.StateHasChanged += this.OnSectionStateChanged;
    }

    public async Task LoadAsync(DmoArticle article, IEnumerable<DmoSection> sections)
    {
        await _root.LoadContextAsync(article, DiodeState.Existing());
        foreach (var section in sections)
        {
            await _sections.AddContextAsync(section.SectionId, section, DiodeState.Existing());
        }
    }

    public DmoSection? GetSection(SectionUid uid)
    {
        if (_sections.TryGetContext(uid, out var context))
            return context.ImmutableItem;

        return null;
    }

    public bool TryGetSection(SectionUid uid, [NotNullWhen(true)] out DmoSection? section)
    {
        if (_sections.TryGetContext(uid, out var context))
        {
            section = context.ImmutableItem;
            return true;
        }

        section = null;
        return false;
    }

    public async ValueTask<DiodeResult<DmoArticle>> DispatchArticleAsync<TAction>(TAction action)
    where TAction : class, IDiodeAction
    {
        if (_root.Context is null)
            return DiodeResult<DmoArticle>.Failure($"No context loaded for DmoArticle");

        var result = await _root.DispatchAsync(action);
        return result;
    }

    public async ValueTask<DiodeResult<DmoSection>> DispatchSectionAsync<TAction>(TAction action)
        where TAction : class, IDiodeAction<SectionUid>
    {
        var result = await _sections.DispatchAsync(action);
        return result;
    }

    private ValueTask OnArticleStateChanged(DiodeContextChangeEventArgs<DmoArticle> e)
    {
        this.StateHasChanged?.Invoke(this, EventArgs.Empty);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<DataResult> MarkArticleForDeletionAsync()
    {
        return await _root.MarkContextForDeletionAsync();
    }

    public async ValueTask<DataResult> MarkSectionForDeletionAsync(SectionUid id)
    {
        return await _sections.MarkContextForDeletionAsync(id);
    }

    private async ValueTask OnSectionStateChanged(DiodeContextChangeEventArgs<DmoSection> e)
    {
        int totalWords = 0;
        foreach (var section in _sections.Contexts.Where(item => !item.Context.State.IsMarkedForDeletion))
        {
            var words = section.Context.ImmutableItem.Content.Split(" ");
            totalWords = totalWords + words.Length;
        }

        var articleMutation = new ArticleUpdateMutation(totalWords);
        await _root.DispatchDelegateAsync(articleMutation.Mutation);
    }

    // Private class to encapsulate the data and mutation to update the Article root 
    private record ArticleUpdateMutation(int WordCount)
    {
        public DiodeAsyncMutationDelegate<DmoArticle> Mutation => (DiodeMutationRequest<DmoArticle> request) =>
        {
            var mutation = request.Item with { WordCount = this.WordCount, LastUpdated = DateTime.Now };
            return Task.FromResult(DiodeResult<DmoArticle>.Success(mutation));
        };
    }
}
