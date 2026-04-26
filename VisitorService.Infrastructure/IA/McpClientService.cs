using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;

public static class McpClientExtensions
{
    public static IServiceCollection AddMcpClient(this IServiceCollection services, string serverUri)
    {
        services.AddHttpClient("McpServerClient").AddHeaderPropagation();

        services.AddScoped<McpClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("McpServerClient");

            var transport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri(serverUri),
                TransportMode = HttpTransportMode.Sse
            }, httpClient);

            return McpClient.CreateAsync(transport).GetAwaiter().GetResult();
        });

        return services;
    }
}