/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public record DataResult
{
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static DataResult Success()
        => new DataResult() { Successful = true};

    public static DataResult Failure(string message)
        => new DataResult() { Successful = false, Message = message};

    public static DataResult Create<T>(DiodeResult<T> result)
        where T : class
        => new DataResult() { Successful = result.Successful , Message = result.Message };
}