using CSharpApp.Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection(nameof(RestApiSettings)).Get<RestApiSettings>();
        services.AddHttpClient<IPlatziApiClient, PlatziApiClient>(client => 
        { 
            client.BaseAddress = new Uri(settings.BaseUrl!);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        return services;
    }
}