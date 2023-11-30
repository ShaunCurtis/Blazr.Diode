namespace Blazr.Diode.Server.Web.Data;

public record DboWeatherForecast
{
    public int Id { get; init; }
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
}

public record WeatherForecast
{
    public WeatherForcastId WeatherForecastId { get; init; }
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; init; }
    public string? Summary { get; init; }
}

public readonly struct WeatherForcastId
{
    public int Id { get; init; }
    public Guid Uid { get; } = Guid.NewGuid();
    public WeatherForcastId(int id)
        => this.Id = id;
    public override bool Equals(object? obj) 
        => obj is WeatherForcastId other && this.Equals(other);
    public override int GetHashCode()
        => Id.GetHashCode();

    public static bool operator ==(WeatherForcastId lhs, WeatherForcastId rhs) => lhs.Id == rhs.Id;

    public static bool operator !=(WeatherForcastId lhs, WeatherForcastId rhs) => lhs.Id != rhs.Id;
}

public readonly record struct Temperature
{
    private readonly int _temperature;
    public int Celcius => _temperature;
    public int Fahrenheit => 32 + (int)(_temperature / 0.5556);
    public Temperature(int value)
        => _temperature = value;
}

public record WeatherForecastMap : IMap<WeatherForecast, DboWeatherForecast>
{
    public DboWeatherForecast Map(WeatherForecast record)
        => MapToDbo(record);

    public WeatherForecast Map(DboWeatherForecast record)
        => MapFromDbo(record);

    public static DboWeatherForecast MapToDbo(WeatherForecast record)
        => new()
        {
            Id = record.WeatherForecastId.Id,
            Date = record.Date,
            TemperatureC = record.Temperature.Celcius,
            Summary = record.Summary,

        };
    public static WeatherForecast MapFromDbo(DboWeatherForecast record)
        => new()
        {
            WeatherForecastId = new(record.Id),
            Date = record.Date,
            Temperature = new(record.TemperatureC),
            Summary = record.Summary,
        };
}
