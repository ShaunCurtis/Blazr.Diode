﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Server.Web.Counter;

public record CounterData : IDiodeEntity
{
    public int Counter { get; init; }
    public int Incrementer { get; init; } = 1;
    public Guid Uid { get; init; }  = Guid.NewGuid();
}
