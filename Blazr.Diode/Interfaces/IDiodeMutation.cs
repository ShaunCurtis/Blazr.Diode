/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public interface IDiodeMutation<T>
    where T : class
{
    string ActionName { get; }
    public DiodeAsyncMutationDelegate<T> Mutation { get; }
}
