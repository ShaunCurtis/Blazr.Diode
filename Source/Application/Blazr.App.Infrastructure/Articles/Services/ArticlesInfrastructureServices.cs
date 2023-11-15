/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class ArticlesInfrastructureServices
{
    public static void AddArticleServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IRecordFilterHandler<WeatherForecast>, WeatherForecastFilterHandler>();
        services.AddTransient<IRecordSortHandler<WeatherForecast>, WeatherForecastSortHandler>();
    }

    public static void AddMappedArticleServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboArticle, DmoArticle>, ArticleMap>();
        services.AddScoped<IDboEntityMap<DboSection, DmoSection>, SectionMap>();
        services.AddScoped<IListRequestHandler, MappedListRequestServerHandler<InMemoryTestDbContext, DboArticle>>();
        services.AddScoped<IItemRequestHandler, MappedItemRequestServerHandler<InMemoryTestDbContext, DboArticle>>();
        services.AddScoped<ICommandHandler, MappedCommandServerHandler<InMemoryTestDbContext, DboArticle>>();
        services.AddScoped<IListRequestHandler, MappedListRequestServerHandler<InMemoryTestDbContext, DboSection>>();
        services.AddScoped<IItemRequestHandler, MappedItemRequestServerHandler<InMemoryTestDbContext, DboSection>>();
        services.AddScoped<ICommandHandler, MappedCommandServerHandler<InMemoryTestDbContext, DboSection>>();

        //services.AddTransient<IRecordFilterHandler<DcoWeatherForecast>, DcoWeatherForecastFilterHandler>();
        //services.AddTransient<IRecordSortHandler<DcoWeatherForecast>, DcoWeatherForecastSortHandler>();

        //services.AddTransient<IRecordFilterHandler<DboWeatherForecast>, DboWeatherForecastFilterHandler>();
        services.AddTransient<IRecordSortHandler<DboArticle>, DboArticleSortHandler>();
    }
}
