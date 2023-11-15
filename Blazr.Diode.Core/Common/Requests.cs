/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Core;

public readonly record struct ItemRequest(object KeyValue);

public readonly record struct CommandRequest<T>(T item, CommandRequestType type);

public enum CommandRequestType
{
    None,
    Create,
    Update,
    Delete,
}
