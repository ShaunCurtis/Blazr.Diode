/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Collections;

namespace Blazr.Diode.Entities;

public class DiodeStoreProvider<T> : IEnumerable<DiodeEntityStore<T>>
    where T : class, IDiodeEntity, new()
{
    protected List<DiodeEntityStore<T>> Stores = new List<DiodeEntityStore<T>>();

    public DiodeEntityStore<T>? GetStore(DiodeUid uid)
        => this.Stores.FirstOrDefault(s => s.Uid == uid);

    public DataResult CreateStore(T item, DiodeState? state = null)
    {
        var store = this.Stores.FirstOrDefault(s => s.Uid == item.Uid);

        if (store is not null)
            return DataResult.Failure($"A store already exists for entity {item.Uid}");

        var newStore = new DiodeEntityStore<T>(item, state ?? DiodeState.New());

        return DataResult.Success();
    }

    public DataResult ClearStore(DiodeUid uid)
    {
        var store = this.Stores.FirstOrDefault(s => s.Uid == uid);

        if (store is null)
            return DataResult.Failure($"No Store exists = {uid}");

        this.Stores.Remove(store);

        return DataResult.Success();
    }

    public IEnumerator<DiodeEntityStore<T>> GetEnumerator()
        => Stores.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
}
