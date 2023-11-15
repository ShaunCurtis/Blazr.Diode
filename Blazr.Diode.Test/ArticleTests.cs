using Blazr.App.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazr.Diode.Test
{
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

        private void LoadTestData(ArticleComposite articleComposite)
        {
            ArticleUid articleUid = new(Guid.NewGuid());
            var article = new DmoArticle() { ArticleId = articleUid, Title = "C# for Beginners", Introduction = "A lesson in learning!" };

            List<DmoSection> sections = new() {
                 new() {ArticleId = articleUid, SectionId = new(Guid.NewGuid()), Title="Chapter 1", Content = "Blah, blah" },
                 new() {ArticleId = articleUid, SectionId = _testSectionUid, Title="Chapter 2", Content = "Blah, blah" },
            };

            articleComposite.Load(article, sections);
        }

        [Fact]
        public async Task TestUpdatingRoot()
        {
            var serviceProvider = GetServiceProvider();

            var composite = serviceProvider.GetRequiredService<ArticleComposite>();
            Assert.NotNull(composite);

            this.LoadTestData(composite);

            var articleEditContext = new DmoArticleEditContext(composite.Article);
            Assert.NotNull(articleEditContext);

            articleEditContext.Title = "C# for Total Beginners";

            var result = await composite.DispatchArticleAsync(articleEditContext);
            //var result = await composite.Root.DispatchDelegateAsync(articleEditContext.Mutation);
            Assert.True(result.Successful);
        }

        [Fact]
        public async Task TestUpdatingSection()
        {
            var serviceProvider = GetServiceProvider();

            var composite = serviceProvider.GetRequiredService<ArticleComposite>();
            Assert.NotNull(composite);

            this.LoadTestData(composite);

            var section = composite.GetSection(_testSectionUid);

            Assert.NotNull(section);

            var sectionEditContext = new DmoSectionEditContext(section);
            Assert.NotNull(sectionEditContext);

            sectionEditContext.Content = "Testing, Testing, Testing";

            var result = await composite.DispatchSectionAsync(sectionEditContext);
            Assert.True(result.Successful);
        }
    }
}