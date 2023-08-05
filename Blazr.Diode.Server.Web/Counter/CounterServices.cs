namespace Blazr.Diode.Server.Web.Counter;

public static class CounterServices
{
    public static void AddCounterServices(this IServiceCollection services)
    {
        services.AddScoped<DiodeStore<CounterData>>();
        services.AddScoped<DiodeDispatcher>();
        services.AddTransient<IDiodeHandler<CounterData,CounterIncrementAction>, CounterIncrementHandler>();
    }
}
