/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Server.Web.Counter;

public record CounterSetIncrementerAction : IDiodeAction<Guid>, IDiodeMutation<CounterData>
{
    public Guid KeyValue { get; init; }
    public string ActionName => $"Set Incrementer";

    public int Incrementer { get; init; } = 1;

    public CounterSetIncrementerAction(Guid uid)
    {
        this.KeyValue = uid;
    }

    public DiodeAsyncMutationDelegate<CounterData> Mutation => (DiodeMutationRequest<CounterData> request) =>
    {
        var mutation = request.Item with { Incrementer = this.Incrementer };

        return Task.FromResult(DiodeResult<CounterData>.Success(mutation));
    };
}
