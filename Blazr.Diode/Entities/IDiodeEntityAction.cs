/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Entities;

/// <summary>
/// Interface defining a Mustation Request
/// </summary>
public interface IDiodeEntityAction
{
    string Name { get; }
    public DiodeUid Uid { get; }
}
