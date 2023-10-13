/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Core;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Diode.Composites;

public class DiodeCompositeProvider<TRootItem, TCollectionItem> : IEnumerable<DiodeComposite<TRootItem, TCollectionItem>>
    where TRootItem : class, IDiodeEntity, new()
    where TCollectionItem : class, IDiodeEntity, new()
{
    private readonly IServiceProvider _serviceProvider;

    public DiodeCompositeProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private List<DiodeComposite<TRootItem, TCollectionItem>> _contexts { get; set; } = new List<DiodeComposite<TRootItem, TCollectionItem>>();

    /// <summary>
    /// Readonly list of Registered Contexts
    /// </summary>
    public IEnumerable<DiodeComposite<TRootItem, TCollectionItem>> Contexts => _contexts.AsEnumerable();

    /// <summary>
    /// Gets the registered context
    /// Will return a null if one doesn't exist
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public DiodeComposite<TRootItem, TCollectionItem>? GetContext(Guid uid)
        => this._contexts.FirstOrDefault(s => s.Uid == uid);

    /// <summary>
    /// Tries to get the registered context
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool TryGetContext(Guid uid, [NotNullWhen(true)] out DiodeComposite<TRootItem, TCollectionItem>? context)
    {
        context = this._contexts.FirstOrDefault(s => s.Uid == uid);
        return context is not null;
    }

    /// <summary>
    /// Creates a context for the provided T 
    /// and adds it to the context list  
    /// </summary>
    /// <param name="item"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public DataResult AddContext(DiodeComposite<TRootItem, TCollectionItem> composite)
    {
        if (_contexts.Any(item => item.Uid == composite.Uid))
            return DataResult.Failure($"A store already exists for entity {composite.Uid}");

        this._contexts.Add(composite);

        return DataResult.Success();
    }

    /// <summary>
    /// Clears the provided context from the context list
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public DataResult ClearContext(Guid uid)
    {
        var store = this._contexts.FirstOrDefault(s => s.Uid == uid);

        if (store is null)
            return DataResult.Failure($"No context exists = {uid}");

        this._contexts.Remove(store);

        return DataResult.Success();
    }

    /// <summary>
    /// Public enumerator for the collection
    /// </summary>
    /// <returns></returns>
    public IEnumerator<DiodeComposite<TRootItem, TCollectionItem>> GetEnumerator()
        => _contexts.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
}
