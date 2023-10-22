/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

/// <summary>
/// Internal object to hold an individual entity's State.
/// Provides the services to transition from the current to the new state
/// </summary>
/// <typeparam name="T"></typeparam>
public class DiodeContext<T>
    where T : class, IDiodeEntity, new()
{
    private Task _task = Task.CompletedTask;
    private TaskCompletionSource<MutationResult<T>> _queueLoopTaskSource = new();
    private readonly Queue<DiodeAsyncMutationDelegate<T>> _mutationQueue = new();
    private T _immutableItem;

    public DiodeState State { get; private set; }

    /// <summary>
    /// Event raised when a context mutates
    /// </summary>
    public event EventHandler<DiodeContextChangeEventArgs<T>>? StateHasChanged;

    public DiodeEntityData<T> AsDiodeEntityData => new(_immutableItem, State);

    /// <summary>
    /// The current entity
    /// </summary>
    public T ImmutableItem
    {
        get => _immutableItem;
        private set
        {
            _immutableItem = value;
        }
    }

    /// <summary>
    /// The Task that represents current state mutation activity
    /// If it not completed then a mutatiuon is taking place
    /// </summary>
    internal Task<MutationResult<T>> QueueLoopTask => _queueLoopTaskSource.Task;

    /// <summary>
    /// Constructor
    /// Requires a state object to populate
    /// </summary>
    /// <param name="entity"></param>
    internal DiodeContext(T item, DiodeState? state = null)
    {
        _immutableItem = item;
        this.State = state ?? DiodeState.New();
        _queueLoopTaskSource.SetResult(MutationResult<T>.NotMutated(this.ImmutableItem));
    }

    /// <summary>
    /// Primary method to apply a mutation to the state
    /// </summary>
    /// <param name="mutation">The DiodeMutationDelegate to run to mutate the object</param>
    /// <returns></returns>
    internal Task<MutationResult<T>> QueueAsync(DiodeAsyncMutationDelegate<T> mutation)
    {
        _mutationQueue.Enqueue(mutation);

        // Start the Queue service if it's not already running
        if (_task.IsCompleted)
            _task = StartQueueLoopAsync();

        return this.QueueLoopTask;
    }

    private async Task StartQueueLoopAsync()
    {
        bool isMutation;

        // local copy to hold the mutation
        var mutation = _immutableItem;

        // create a new running task to track the process
        _queueLoopTaskSource = new();

        // loop through thw queued mutations
        while (_mutationQueue.Count > 0)
        {
            var action = _mutationQueue.Dequeue();
            var result = await action.Invoke(new(mutation));

            // Apply the mutation if successful
            if (result.Successful && result.Item is not null)
            {
                mutation = result.Item;
            }
        }

        // check if we have actually mutated
        isMutation = _immutableItem != mutation;

        // If we have actually mutated the object set the value and state
        if (isMutation)
        {
            _immutableItem = mutation;
            this.State = this.State.Mutated;
        }

        // complete the task tracking the process
        _queueLoopTaskSource?.SetResult(new MutationResult<T>(this.ImmutableItem, isMutation));

        // notify only if we have effected a mutation
        if (isMutation)
            NotifyStateHasChanged();
    }

    internal void MarkAsPersisted()
    {
        this.State = this.State.Persisted;
        this.NotifyStateHasChanged();
    }

    internal void MarkForDeletion()
    {
        this.State = this.State.MarkForDeletion;
        this.NotifyStateHasChanged();
    }

    internal void NotifyStateHasChanged()
    {
        this.StateHasChanged?.Invoke(this, new DiodeContextChangeEventArgs<T>(this));
    }
}
