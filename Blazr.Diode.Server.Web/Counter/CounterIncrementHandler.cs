namespace Blazr.Diode.Server.Web.Counter;

public class CounterIncrementHandler : IDiodeHandler<CounterData, CounterIncrementAction>
{
    public CounterIncrementAction Action { get; set; } = default!;

    public Task<DiodeMutationResult<CounterData>> MutateAsync(DiodeMutationRequest<CounterData> request)
    {
        ArgumentNullException.ThrowIfNull(Action);

        var result = request.Item with { Counter = request.Item.Counter + Action.IncrementBy };
        return Task.FromResult(DiodeMutationResult<CounterData>.Success(result));
    }
}
