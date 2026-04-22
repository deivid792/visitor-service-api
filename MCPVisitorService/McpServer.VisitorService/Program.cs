using System.Xml.Schema;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

var serverInfo = new Implementation {Name = "DotnetMCPSeerver", Version = "1.0.0"};

builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5057/");
});

builder.Services.AddHttpContextAccessor();

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddMcpServer(mcp =>
    {
        mcp.ServerInfo = serverInfo;
    })
    .WithHttpTransport()
    .WithTools<AuthRegisterTools>()
    .WithTools<CreateVisitsTools>()
    .WithTools<ManagerVisitsTools>();

var app = builder.Build();

app.MapMcp();

await app.RunAsync();
