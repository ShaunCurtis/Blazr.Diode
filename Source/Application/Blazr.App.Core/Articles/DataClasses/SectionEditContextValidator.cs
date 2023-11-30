/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class SectionEditContextValidator : AbstractValidator<DmoSectionEditContext>
{
    public SectionEditContextValidator()
    {
        this.RuleFor(p => p.Title)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
