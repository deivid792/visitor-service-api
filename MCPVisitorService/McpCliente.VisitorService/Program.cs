using System.ClientModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;


var builder = WebApplication.CreateBuilder(args);


const string groqApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");

var openAIClient = new OpenAIClient(
    new ApiKeyCredential(groqApiKey),
    new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai/v1") }
);

var chatClient = openAIClient.GetChatClient("llama-3.3-70b-versatile");

IChatClient client = chatClient.AsIChatClient();

builder.Services.AddChatClient(client)
    .UseFunctionInvocation(configure: client =>
    {
        client.MaximumIterationsPerRequest = 1;
    });

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MCP_Chat_";
});


builder.Services.AddScoped<ChatHistoryService>();

builder.Services.AddHttpClient("McpServerClient")
    .AddHeaderPropagation();

builder.Services.AddHeaderPropagation(options =>
{
    options.Headers.Add("Authorization");
});

builder.Services.AddScoped<McpClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("McpServerClient");

    
    var transport = new HttpClientTransport(new HttpClientTransportOptions
    {
        Endpoint = new Uri("http://localhost:5000/sse"),
        TransportMode = HttpTransportMode.Sse
    }, httpClient);

    return McpClient.CreateAsync(transport).GetAwaiter().GetResult();
});

builder.Services.AddCors(options => options.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

string aiInstructions = await File.ReadAllTextAsync("ai-instructions.md");

var app = builder.Build();
app.UseHeaderPropagation();
app.UseCors();


app.MapPost("/chat", async (ChatRequest request, IChatClient chatClient, McpClient mcpClient, ChatHistoryService historyService) =>
{
    string sessionId = request.UserId ?? "usuario_padrao";
    var history = await historyService.GetHistoryAsync(sessionId);

    var messages = new List<ChatMessage>
    {
        new ChatMessage(ChatRole.System, aiInstructions)
    };

    if (history != null)
    {
        messages.AddRange(history.Where(m => m.Role != ChatRole.System).TakeLast(6));
    }

    messages.Add(new ChatMessage(ChatRole.User, request.Prompt));

    IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
    var options = new ChatOptions { Tools = tools.Cast<AITool>().ToList() };

    var response = await chatClient.GetResponseAsync(messages, options);

    messages.Add(new ChatMessage(ChatRole.Assistant, response.Text ?? ""));
    await historyService.SaveHistoryAsync(sessionId, messages);

    return Results.Ok(new { answer = response.Text });
});

app.Run("http://localhost:5001");

public record ChatRequest(string Prompt, string? UserId);