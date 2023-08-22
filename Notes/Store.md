# Diode Store

The store:

1. Holds the *Single Version of the Truth*.
2. Mutates the *Single Version of the Truth*.
3. Notifies when a mutation is complete. 

The class skeleton looks like this:

```csharp
public class DiodeStore<T>
    where T : class, new()
{
    // Event that any listener can subscribe to for mutations
    public event EventHandler<DiodeStateChangeEventArgs>? StateHasChanged;

    // The current version of the truth
    public T Item
    {
        get => _item;
        private set
        {
            _item = value;
        }
    }

    // Queue Task.  If it's Completed then no queue event is currently running
    public Task<T> QueueLoopTask => _queueLoopTaskSource.Task;

    // Method to queue a mutation event.  If the queue isn't running it will start the queue
    public Task<T> QueueAsync(DiodeMutationDelegate<T> mutation);
}
```

The beating heart of the store is the Store queue, and the queue loop.

The queue loop only runs when there are active items in the queue.  It's on/off state is controlled by a `TaskCompletionSource`.

The queue is an private `readonly` `Queue<T>`.

```csharp
    private readonly Queue<DiodeMutationDelegate<T>> _mutationQueue = new();
```

The `TaskCompletionSource<T>` that provides the async context for the loop.

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

It gets the action from the queue:

```csharp
    var acton = _mutationQueue.Dequeue();
```

Invokes it:

```csharp
    var result = await acton.Invoke(new(_item));

```

And if it's successful, assigns the new value to `Item`

```csharp
    if (result.Successful && result.Item is not null)
        this.Item = result.Item;
```

Once it's processed all the items in the queue it Sets the Task source to complete:

```csharp
    _queueTaskSource?.SetResult(this.Item);
```

And then raised the `StateHasChanged` event.

```csharp
    NotifyStateHasChanged(this.Item);
```

```csharp
    private void NotifyStateHasChanged(T? item)
        => this.StateHasChanged?.Invoke(this, new DiodeStateChangeEventArgs(item));
```


