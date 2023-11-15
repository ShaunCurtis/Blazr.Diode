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

    public DmoArticle Article => _root.Context.ImmutableItem;
    public IEnumerable<DmoSection> Sections => _sections.AsList.AsEnumerable();

    private DiodeSingletonCompositeProvider<DmoArticle> _root;
    private DiodeCollectionCompositeProvider<SectionUid, DmoSection> _sections;

    public ArticleComposite(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _root = new(_serviceProvider);
        _sections = new(_serviceProvider);
    }

    public void Load(DmoArticle article, IEnumerable<DmoSection> sections)
    {
        _root.AddContext(article);
        foreach (var section in sections)
        {
            _sections.CreateContext(section.SectionId, section);
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
}
