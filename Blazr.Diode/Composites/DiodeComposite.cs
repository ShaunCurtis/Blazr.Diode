/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core;

namespace Blazr.Diode.Composites;

public abstract class DiodeComposite<TRootItem, TCollectionItem> : IDiodeEntity
    where TRootItem : class, IDiodeEntity, new()
    where TCollectionItem : class, IDiodeEntity, new()
{
    private readonly IServiceProvider _serviceProvider;
    private bool _loaded;

    private DiodeCompositeData<TRootItem, TCollectionItem>? _baseData;

    protected DiodeContextProvider<TRootItem> rootProvider;
    protected DiodeContextProvider<TCollectionItem> itemsProvider;

    public DiodeContext<TRootItem>? Root => rootProvider.FirstOrDefault();

    public IEnumerable<DiodeContext<TCollectionItem>> Items => this.itemsProvider.AsEnumerable();

    public IEnumerable<DiodeEntityData<TCollectionItem>> ItemsAsEntityData => itemsProvider.AsEntityData; 

    public Guid Uid => this.Root?.Uid ?? Guid.Empty;

    public EntityUid EntityUid => new(this.Uid);

    public event EventHandler<DiodeStateChangeEventArgs>? StateHasChanged;

    public DiodeComposite(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    
        var rootProvider = ActivatorUtilities.CreateInstance<DiodeContextProvider<TRootItem>>(_serviceProvider);
        var itemsProvider = ActivatorUtilities.CreateInstance<DiodeContextProvider<TCollectionItem>>(_serviceProvider);

        ArgumentNullException.ThrowIfNull(rootProvider);
        ArgumentNullException.ThrowIfNull(itemsProvider);

        this.rootProvider = rootProvider;
        this.itemsProvider = itemsProvider;
    }

    public ValueTask<DataResult> LoadAsync(DiodeCompositeData<TRootItem, TCollectionItem> aggregate)
    {
        if (_loaded)
            return ValueTask.FromResult(DataResult.Failure($"The Composite has already been loaded with data."));

        _baseData = aggregate;

        var result = this.rootProvider.CreateContext(aggregate.Root.Data, aggregate.Root.State);

        if (!result.Successful)
            return ValueTask.FromResult(DataResult.Failure(result.Message ?? "Could not create the composite root."));

        foreach (var item in _baseData.Items)
        {
            var collectionResult = itemsProvider.CreateContext(item.Data, item.State);
            if (!collectionResult.Successful)
                return ValueTask.FromResult(DataResult.Failure(collectionResult.Message ?? "Could not create a composite collection context."));
        }

        _loaded = true;
        return ValueTask.FromResult(DataResult.Success());
    }

    /// <summary>
    /// Adds a new Root Item
    /// You can only do this when one doesn't already exist i.e. the object is in a new state
    /// At all other times you need to Update the Root item with an IDiodeAction
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<DiodeResult<DiodeContext<TRootItem>>> AddRootAsync(TRootItem item)
    {
        if (_loaded || rootProvider.Count() > 0)
            return DiodeResult<DiodeContext<TRootItem>>.Failure($"The Composite has already been loaded with root data.  You must modify the root.");

        var result = rootProvider.CreateContext(item, DiodeState.New());
        if (!result.Successful)
        {
            await this.NotifyStateHasChangedAsync();
            _loaded = true;
        }

        return result;
    }

    /// <summary>
    /// Mutates the root item based on the supplied IDiodeAction
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task<DataResult> UpdateRootAsync(IDiodeAction action)
    {
        var result = await rootProvider.DispatchAsync(action);

        await this.NotifyStateHasChangedAsync();
        return DataResult.Create(result);
    }

    /// <summary>
    /// Marks the root and thus the whole context for deletion
    /// You need to persist the context to effect the actual deletion on the data store
    /// </summary>
    /// <returns></returns>
    public async Task<DataResult> DeleteRootAsync()
    {
        await this.NotifyStateHasChangedAsync();
        return this.rootProvider.MarkContextForDeletion(this.Uid);
    }

    /// <summary>
    /// Mutates the TCollectionItem based on the supplied IDiodeAction
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task<DataResult> UpdateItemAsync(IDiodeAction action)
    {
        var result = await itemsProvider.DispatchAsync(action);

        await this.NotifyStateHasChangedAsync();
        return DataResult.Create(result);
    }

    /// <summary>
    /// Adds a new Invoice item to the context
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<DiodeResult<DiodeContext<TCollectionItem>>> AddItemAsync(TCollectionItem item)
    {
        await this.NotifyStateHasChangedAsync();
        return itemsProvider.CreateContext(item);
    }

    /// <summary>
    /// Marks an invoice item for deletion
    /// You need to persist the Invoice Composite to effect the deletion in the data store
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public async Task<DataResult> DeleteItemAsync(Guid uid)
    {
        await this.NotifyStateHasChangedAsync();
        return this.rootProvider.MarkContextForDeletion(uid);
    }

    /// <summary>
    /// Marks the invoice and all invoice items as existing and clean [not mutated]
    /// And removes all the deleted invoice items
    /// </summary>
    /// <returns></returns>
    public async Task MarkAsPersistedAsync()
    {
        // mark the root item as persisted
        foreach (var item in rootProvider)
            rootProvider.MarkContextAsPersisted(item.Uid);

        // Get all the items marked for deletion 
        var itemsToDelete = itemsProvider.Where(item => item.State.IsMarkedForDeletion).Select(item => item.Uid).ToList();

        // And remove them
        foreach (var item in itemsToDelete)
            rootProvider.ClearContext(item);

        // mark all the remaining items as persisted
        foreach (var item in itemsProvider)
            rootProvider.MarkContextAsPersisted(item.Uid);

        await this.NotifyStateHasChangedAsync();
    }

    /// <summary>
    /// Carries out root calculations and
    /// raises the StateHasChanged Event
    /// </summary>
    /// <returns></returns>
    public async Task NotifyStateHasChangedAsync()
    {
        await this.ExecuteRootCalculationsAsync();
        this.StateHasChanged?.Invoke(this, new DiodeStateChangeEventArgs(this.Uid, this));
    }

    /// <summary>
    /// Updates the root based on changes in item collection
    /// And mutates the root if it has changed
    /// </summary>
    /// <returns></returns>
    protected virtual Task ExecuteRootCalculationsAsync()
        => Task.CompletedTask;
}