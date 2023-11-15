/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Core;

public readonly record struct ListQueryResult<T>(IEnumerable<T> items, bool Success, string? Message = null );

public readonly record struct ItemQueryResult<T>(T? item, bool Success, string? Message = null);

public readonly record struct CommandResult(bool Success, string? Message = null);
