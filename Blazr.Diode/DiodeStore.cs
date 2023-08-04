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
public class DiodeStore<T>
    where T : class, new()
{
    private Task _queueTask = Task.CompletedTask;
    private TaskCompletionSource<T> _taskCompletionSource = new();
    private readonly Queue<DiodeMutationDelegate<T>> _mutationQueue = new();
    private T _item;

    /// <summary>
    /// Event raised when an entity mutates
    /// </summary>
    public event EventHandler<DiodeStateChangeEventArgs>? StateHasChanged;

    /// <summary>
    /// The current OwsEntityData object for the entity
    /// </summary>
    public T Item { get; private set; }

    /// <summary>
    /// The Task that represents current state mutation activity
    /// If it not completed then a mutatiuon is taking place
    /// </summary>
    public Task<T> StateMutationTask => _taskCompletionSource.Task;

    /// <summary>
    /// The last access/mutation activity on the State
    /// Used by the mian state object to determine when to dispose the entity state
    /// </summary>
    internal DateTimeOffset LastActivity = DateTimeOffset.Now;

    /// <summary>
    /// Constructor
    /// Requires a state object to populate
    /// </summary>
    /// <param name="entity"></param>
    public DiodeStore(T item)
    {
        _item = item;
        this.Item = _item;
        _taskCompletionSource.SetResult(this.Item);
        this.LastActivity = DateTimeOffset.Now;
    }

    /// <summary>
    /// Primary method to apply a mutation to the state
    /// </summary>
    /// <param name="mutation">The OwsStateMutationDelegate to run to mutate the object</param>
    /// <returns></returns>
    public Task<T> DispatchAsync(DiodeMutationDelegate<T> mutation)
    {
        _mutationQueue.Enqueue(mutation);

        // Start the Queue service if it's not already running
        if (_queueTask.IsCompleted)
            _queueTask = ServiceQueueAsync();

        return this.StateMutationTask;
    }

    private async Task ServiceQueueAsync()
    {
        _taskCompletionSource = new();

        while (_mutationQueue.Count > 0)
        {
            var acton = _mutationQueue.Dequeue();
            var result = await acton.Invoke(new(_item));
            if (result.Successful && result.Item is not null)
            {
                _item = result.Item;
                this.Item = _item;
            }
        }

        _taskCompletionSource?.SetResult(this.Item);
        LastActivity = DateTimeOffset.Now;
        
        NotifyStateHasChanged(this.Item);
    }

    private void NotifyStateHasChanged(T? item)
        => this.StateHasChanged?.Invoke(this, new DiodeStateChangeEventArgs(item));
}
