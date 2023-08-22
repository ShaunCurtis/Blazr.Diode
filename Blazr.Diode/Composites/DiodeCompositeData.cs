/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Aggregates;

/// <summary>
/// A class to represent data ibn a classic Aggregate with a root item and a collection
/// An invoice and invoice items or shopping basket and basket items are examples 
/// </summary>
/// <typeparam name="TRootItem"></typeparam>
/// <typeparam name="TCollectionItem"></typeparam>
public record DiodeCompositeData<TRootItem, TCollectionItem> : IDiodeEntity
    where TRootItem : class, IDiodeEntity, new()
    where TCollectionItem : class, IDiodeEntity, new()
{
    private IEnumerable<DiodeEntityData<TCollectionItem>> _items;

    public Guid Uid { get; init; }

    public IEnumerable<DiodeEntityData<TCollectionItem>> Items => _items.AsEnumerable();

    public DiodeEntityData<TRootItem> Root { get; init; }

    public DiodeCompositeData(Guid uid, DiodeEntityData<TRootItem> root, IEnumerable<DiodeEntityData<TCollectionItem>> items)
    {
        _items = items;
        this.Root = root;
        this.Uid = uid;
    }
}
