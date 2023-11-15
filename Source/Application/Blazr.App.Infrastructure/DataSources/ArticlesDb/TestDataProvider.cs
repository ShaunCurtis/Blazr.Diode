/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed class TestDataProvider
{
    internal IEnumerable<DboArticle> Articles => _articles.AsEnumerable();
    internal IEnumerable<DboSection> Sections => _sections.AsEnumerable();

    private List<DboArticle> _articles = new List<DboArticle>();
    private List<DboSection> _sections = new List<DboSection>();

    public TestDataProvider()
    {
        this.Load();
    }

    private void Load()
    {
        _articles = new()
        {
            new() { ArticleId = Guid.NewGuid(),
                Title = "Programming C#",
                Introduction = "A beginners guid to programming C#" },

            new() { ArticleId = Guid.NewGuid(),
                Title = "A Blazor Primer",
                Introduction = "A beginners guid to Blazor" },
        };

        foreach (var article in _articles)
        {
            _sections.Add(new()
            {
                SectionId = Guid.NewGuid(),
                ArticleId = article.ArticleId,
                Title = "Chapter 1",
                Content = "Blah, blah"
            });

            _sections.Add(new()
            {
                SectionId = Guid.NewGuid(),
                ArticleId = article.ArticleId,
                Title = "Chapter 1",
                Content = "Blah, blah"
            });
        }
    }


    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var dboArticles = dbContext.Set<DboArticle>();
        var dboSections = dbContext.Set<DboSection>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (dboArticles.Count() == 0)
            dbContext.AddRange(_articles);

        if (dboSections.Count() == 0)
            dbContext.AddRange(this._sections);

        dbContext.SaveChanges();
    }

    private static TestDataProvider? _provider;

    public static TestDataProvider Instance()
    {
        if (_provider is null)
            _provider = new TestDataProvider();

        return _provider;
    }

}