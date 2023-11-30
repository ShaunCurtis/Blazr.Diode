/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class ArticleEditContextValidator : AbstractValidator<DmoArticleEditContext>
{
    public ArticleEditContextValidator()
    {
        this.RuleFor(p => p.Introduction)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
