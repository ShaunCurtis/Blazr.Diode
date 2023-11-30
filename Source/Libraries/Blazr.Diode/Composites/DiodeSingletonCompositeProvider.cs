/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Composites;

public class DiodeSingletonCompositeProvider<T>
    where T : class, new()
{
    private readonly IServiceProvider _serviceProvider;
    private DiodeContext<T>? _context;

    public DiodeSingletonCompositeProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public DiodeEntityData<T> AsEntityData =>
        _context?.AsDiodeEntityData ?? throw new DiodeCompositeException("Trying to access the Diode Context before it has been configured.");

    /// <summary>
    /// Returns the current context
    /// Raises exception is the context hasn't yet been loaded
    /// </summary>
    public DiodeContext<T> Context => 
        _context ?? throw new DiodeCompositeException("Trying to access the Diode Context before it has been configured.");

    public Func<DiodeContextChangeEventArgs<T>, ValueTask>? StateHasChanged;

    /// <summary>
    /// Adds a context to an empty provider
    /// </summary>
    /// <param name="item"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<DiodeContext<T>>> LoadContextAsync(T item, DiodeState? state = null)
    {
        if (_context is not null)
            return DiodeResult<DiodeContext<T>>.Failure("You can't overwrite an existing context.");

        _context = new DiodeContext<T>(item, state ?? DiodeState.New());
        await this.OnStateChangeAsync(_context);

        return DiodeResult<DiodeContext<T>>.Success(_context);
    }

    /// <summary>
    /// Applies the dispatched mutation action to the context
    /// </summary>
    /// <typeparam name="TAction"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<T>> DispatchAsync<TAction>(TAction action)
        where TAction : class, IDiodeAction
    {
        // deal with a null store
        if (_context is null)
            return DiodeResult<T>.Failure($"No Context is currently loaded.");

        MutationResult<T> result;

        // check if TAction is a self contained Mutation Action
        if (action is IDiodeMutation<T> mutationAction)
        {
            // Queues the Mutation's DiodeMutationDelegate onto the store's mutation queue
            result = await _context.QueueAsync(mutationAction.Mutation);

            if (result.IsMutated)
                await this.OnStateChangeAsync(_context);

            return DiodeResult<T>.Success(result.Item);
        }

        // We have an action that requires a handler
        // Gets the DI registered action from the DI Provider
        var handler = _serviceProvider.GetService<IDiodeHandler<T, TAction>>();

        // deal with a null Handler
        if (handler is null)
            return DiodeResult<T>.Failure($"Could not locate a registered handler for {typeof(TAction).Name}");

        handler.Action = action;

        // Queues the DiodeMutationDelegate onto the context's mutation queue
        result = await _context.QueueAsync(handler.Mutation);

        if (result.IsMutated)
            await this.OnStateChangeAsync(_context);

        return DiodeResult<T>.Success(result.Item);
    }

    /// <summary>
    /// Applies the provided mutation delegate to the context 
    /// </summary>
    /// <param name="mutationDelegate"></param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public async ValueTask<DiodeResult<T>> DispatchDelegateAsync(DiodeAsyncMutationDelegate<T> mutationDelegate)
    {
        // deal with a null store
        if (_context is null)
            return DiodeResult<T>.Failure($"Could not locate a registered context for {typeof(T).Name}.");

        var result = await _context.QueueAsync(mutationDelegate);

        if (result.IsMutated)
            await this.OnStateChangeAsync(_context);

        return DiodeResult<T>.Success(result.Item);
    }

    /// <summary>
    /// Resets the state on the context
    /// </summary>
    /// <param name="uid"></param>
    public async ValueTask<DataResult> MarkContextAsPersistedAsync()
    {
        if (_context is null)
            return DataResult.Failure($"No Context exists.");

        _context.MarkAsPersisted();
        await this.OnStateChangeAsync(_context);
        return DataResult.Success();

    }

    /// <summary>
    /// Sets the deletion flag on the context
    /// T record will be deleted from the data store when the context is next persisted
    /// </summary>
    /// <param name="uid"></param>
    public async ValueTask< DataResult> MarkContextForDeletionAsync()
    {
        if (_context is null)
            return DataResult.Failure($"No Context exists.");

        _context.MarkForDeletion();
        await this.OnStateChangeAsync(_context);
        return DataResult.Success();
    }

    private async ValueTask OnStateChangeAsync(DiodeContext<T> sender)
    {
        if (this.StateHasChanged is not null)
        {
            await this.StateHasChanged.Invoke(new DiodeContextChangeEventArgs<T>(null, sender));
        }
    }
}
