# Diode Store

The store:

1. Holds the *Single Version of the Truth*.
2. Mutates the store item.
3. Notifies when a mutation is complete. 

The class skeleton looks like this:

```csharp
public class DiodeStore<T>
    where T : class, new()
{
    // Event that any listener can subscribe to for mutations
    public event EventHandler<DiodeStateChangeEventArgs>? StateHasChanged;
    // The current version of the truth
    public T Item { get; private set; }
    // Queue Task.  If it's Completed then no queue event is currently running
    public Task<T> QueueTask => _queueTaskSource.Task;

    // Method to queue a mutation event.  If the queue isn't running it will start the queue
    public Task<T> QueueAsync(DiodeMutationDelegate<T> mutation);
}
```

At the heart of the store is the store queue, and the store loop process.

```csharp
    private readonly Queue<DiodeMutationDelegate<T>> _mutationQueue = new();
```

And a `TaskCompletionSource<T>` that provides the async context for the loop.

```csharp
    private TaskCompletionSource<T> _queueLoopTaskSource = new();
```

When `QueueAsync` is called:

```csharp
    public Task<T> QueueAsync(DiodeMutationDelegate<T> mutation)
```

It:

1. Adds the provided `DiodeMutationDelegate<T>` to the queue:

```csharp
   _mutationQueue.Enqueue(mutation);
```

2. Checks the queue status: is `_queueLoopTask` completed?  It starts it if it isn't and assigns the Task it returns to `_queueLoopTask`.

```csharp
    if (_queueLoopTask.IsCompleted)
        _queueLoopTask = StartQueueLoopAsync();
```

3. Finally it returns the `QueueLoopTask`, the task associated with `_queueLoopTaskSource`.

```csharp
        return this.QueueTask;
```

The key point to note here is that the task returned to the process that queued the delegate gets the task associated with thw queue process.  It doesn't complete until the queue process completes, and therefore, if it's a UI event, it doesn't complete and update the UI until the queue completes.

### StartQueueLoopAsync

`StartQueueLoopAsync` creates a new `TaskCompletionSource<T>`:

```csharp
    _queueLoopTaskSource = new();
```

And then loops through the queue processing routine.

It gets thw action from the queue:

```csharp
    var acton = _mutationQueue.Dequeue();
```

Invokes it 
```csharp
    var result = await acton.Invoke(new(_item));

```

And if it's successful, assigns the new value to 
```csharp
    if (result.Successful && result.Item is not null)
        this.Item = result.Item;
```




When the status of the task associated with this *Not Completed*, the queue process is running i.e. it is processing a `DiodeMutationDelegate` in the queue.  `_taskCompletionSource` is set to completed, and the Task associated completes, when the queue process has completed and the queue is empty.



  There queue service is controlled by a `TaskCompletionSource`.  when the queue service stsrts it sets `_taskCompletionSource` to a new running task.  it then runs any `DiodeMutationDelegate` delegates on the queue and sets `_taskCompletionSource` to completed.  Any new delegates queued while the queue servie loop is running are run in the batch.

```csharp
    private Task _queueTask = Task.CompletedTask;
    private TaskCompletionSource<T> _taskCompletionSource = new();
    private readonly Queue<DiodeMutationDelegate<T>> _mutationQueue = new();
    private T _item;

    private async Task StartQueueAsync()
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

```
