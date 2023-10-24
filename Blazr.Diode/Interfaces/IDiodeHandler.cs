/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public interface IDiodeHandler<K, T, TAction>
    where T : class
    where TAction : class, IDiodeAction<K>
{
    public TAction? Action { get; set; }
    public DiodeAsyncMutationDelegate<T> Mutation { get; }
}

public interface IDiodeHandler<T, TAction>
    where T : class
    where TAction : class, IDiodeAction
{
    public TAction? Action { get; set; }
    public DiodeAsyncMutationDelegate<T> Mutation { get; }
}
