/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

/// <summary>
/// A record that represents the data within a Diode context
/// Used in composite data transfer objects to transport data to and from the dats store
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Data"></param>
/// <param name="State"></param>
public record DiodeEntityData<T>(T Data, DiodeState State)
    where T : class, new();
