using Blazr.App.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazr.Diode.Test;

public class ArticleTests
{
    private SectionUid _testSectionUid = new(Guid.NewGuid());

    private IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddTransient<ArticleComposite>();
        services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        return provider!;
    }

    private async ValueTask LoadTestDataAsync(ArticleComposite articleComposite)
    {
        ArticleUid articleUid = new(Guid.NewGuid());
        var article = new DmoArticle()
        {
            ArticleId = articleUid,
            Title = "C# for Beginners",
            Introduction = "A lesson in learning!",
            WordCount = 4,
        };

        List<DmoSection> sections = new() {
             new() {ArticleId = articleUid, SectionId = new(Guid.NewGuid()), Title="Chapter 1", Content = "Blah, blah" },
             new() {ArticleId = articleUid, SectionId = _testSectionUid, Title="Chapter 2", Content = "Blah, blah" },
        };

        await articleComposite.LoadAsync(article, sections);

    }

    [Fact]
    public async Task TestUpdatingRoot()
    {
        var serviceProvider = GetServiceProvider();

        var composite = serviceProvider.GetRequiredService<ArticleComposite>();
        Assert.NotNull(composite);

        await this.LoadTestDataAsync(composite);

        var articleEditContext = new DmoArticleEditContext(composite.Article);
        Assert.NotNull(articleEditContext);

        var newTitle = "C# for Total Beginners";
        articleEditContext.Title = newTitle;

        var result = await composite.DispatchArticleAsync(articleEditContext);
        Assert.True(result.Successful);
        Assert.Equal(newTitle, composite.Article.Title);
    }

    [Fact]
    public async Task TestUpdatingSection()
    {
        var serviceProvider = GetServiceProvider();

        var composite = serviceProvider.GetRequiredService<ArticleComposite>();
        Assert.NotNull(composite);

        await this.LoadTestDataAsync(composite);

        var section = composite.GetSection(_testSectionUid);

        Assert.NotNull(section);

        var sectionEditContext = new DmoSectionEditContext(section);
        Assert.NotNull(sectionEditContext);

        sectionEditContext.Content = "Testing, Testing, Testing, Testing, Testing, Testing";

        var result = await composite.DispatchSectionAsync(sectionEditContext);
        Assert.True(result.Successful);
    }

    [Fact]
    public async Task TestAddingSection()
    {
        var serviceProvider = GetServiceProvider();

        var composite = serviceProvider.GetRequiredService<ArticleComposite>();
        Assert.NotNull(composite);

        await this.LoadTestDataAsync(composite);

        // Creates a sectionEditContext with a new Section
        var sectionEditContext = new DmoSectionEditContext(composite.Article.ArticleId);
        Assert.NotNull(sectionEditContext);

        // Update the section
        sectionEditContext.Title = "Test adding new Section";
        sectionEditContext.Content = "Adding a further five words";

        // Get the record from the edit context
        var section = sectionEditContext.AsRecord;
        var sectionId = section.SectionId;

        // Dispatch the section
        var result = await composite.DispatchSectionAsync(sectionEditContext);
        Assert.True(result.Successful);

        Assert.Equal(3, composite.Sections.Count());
        Assert.Equal(section, composite.GetSection(sectionId));
    }

    [Fact]
    public async Task TestDeleteASection()
    {
        var serviceProvider = GetServiceProvider();

        var composite = serviceProvider.GetRequiredService<ArticleComposite>();
        Assert.NotNull(composite);

        await this.LoadTestDataAsync(composite);

        var section = composite.GetSection(_testSectionUid);
        Assert.NotNull(section);
        var sectionId = section.SectionId;

        var result = await composite.MarkSectionForDeletionAsync(sectionId);
        Assert.True(result.Successful);
    }


    [Fact]
    public async Task TestDeleteArticle()
    {
        var serviceProvider = GetServiceProvider();

        var composite = serviceProvider.GetRequiredService<ArticleComposite>();
        Assert.NotNull(composite);

        await this.LoadTestDataAsync(composite);

        var result = await composite.MarkArticleForDeletionAsync();
        Assert.True(result.Successful);
    }
}