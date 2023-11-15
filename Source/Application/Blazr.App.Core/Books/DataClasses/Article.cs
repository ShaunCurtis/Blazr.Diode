/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class Article
{
    public Guid ArticleId { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public List<Section> Sections { get; set; } = new List<Section>();
}
