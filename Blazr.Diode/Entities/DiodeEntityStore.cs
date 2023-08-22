/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Entities;

public class DiodeEntityStore<T> : DiodeStore<T>
where T : class, IDiodeEntity, new()
{
    public DiodeUid Uid => Item.Uid;
    public DiodeState State { get; private set; } = new DiodeState();

    public DiodeEntityStore()
        :base() 
    {}

    public DiodeEntityStore(T item)
    : base(item)
    { 
        this.State = DiodeState.New();
    }

    public DiodeEntityStore(T item, DiodeState state)
    : base(item)
    {
        this.State = state;
    }

    /// <summary>
    /// Method to load the initial Item with state
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isNew"></param>
    /// <returns></returns>
    public virtual async Task<DiodeResult<T>> LoadAsync(T item, bool isNew = false)
    {
        if (this.IsMutated)
            return DiodeResult<T>.Failure($"You can't overload an entity store once it's been mutated");

        await this.QueueAsync(
            (request) => Task.FromResult(DiodeResult<T>.Success(item)) 
            );

        State = new DiodeState(isNew);

        return DiodeResult<T>.Success(this.Item);
    }

    protected override Task OnQueueCompleted()
    {
        this.State = this.State.Mutated;

        return base.OnQueueCompleted();
    }

    public void MarkAsPersisted()
        => this.State = this.State.Persisted;

    public void MarkForDeletion()
        => this.State = this.State.MarkForDeletion;
}
