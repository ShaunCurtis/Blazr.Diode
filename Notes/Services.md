# Diode Services

The simplest way to manage services for each entity is to create an `IServiceCollection` extension method to register the necessary services.

For counter this looks like this:

```csharp
public static class CounterServices
{
    public static void AddCounterServices(this IServiceCollection services)
    {
        // Only register once
        services.AddScoped<DiodeDispatcher>();

        services.AddScoped<DiodeStore<CounterData>>();
        services.AddTransient<IDiodeHandler<CounterData,CounterIncrementAction>, CounterIncrementHandler>();
    }
}
```

This is then used in `Program`:

```csharp
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddCounterServices();
```
