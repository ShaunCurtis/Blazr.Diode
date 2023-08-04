/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public interface IDiodeHandler<T, TAction>
    where T : class
    where TAction : class, IDiodeAction
{
    public Task<DiodeMutationResult<T>> MutateAsync(DiodeMutationRequest<T> request);
}
