/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Server.Web.Counter;

public class CounterIncrementHandler : IDiodeHandler<Guid, CounterData, CounterIncrementAction>
{
    public CounterIncrementAction? Action { get; set; } = default;

    public DiodeAsyncMutationDelegate<CounterData> Mutation => (DiodeMutationRequest<CounterData> request) =>
    {
        var mutation = request.Item with { Counter = request.Item.Counter + request.Item.Incrementer };

        return Task.FromResult(DiodeResult<CounterData>.Success(mutation));
    };
}