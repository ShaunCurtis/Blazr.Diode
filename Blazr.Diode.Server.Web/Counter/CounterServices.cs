namespace Blazr.Diode.Server.Web.Counter;

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
