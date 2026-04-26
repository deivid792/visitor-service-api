using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

public class ChatService
{
    private readonly IChatClient _chatClient;
    private readonly McpClient _mcpClient;
    private readonly ChatHistoryService _historyService;
    private readonly string _instructions;

    public ChatService(IChatClient chatClient, McpClient mcpClient, ChatHistoryService historyService)
    {
        _chatClient = chatClient;
        _mcpClient = mcpClient;
        _historyService = historyService;
        _instructions = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "ai-instructions.md"));
    }

    public async Task<string> GetResponseAsync(string userId, string prompt)
    {
        var history = await _historyService.GetHistoryAsync(userId);
        
        var messages = new List<ChatMessage> { new ChatMessage(ChatRole.System, _instructions) };
        if (history != null) messages.AddRange(history.TakeLast(6));
        messages.Add(new ChatMessage(ChatRole.User, prompt));

        var tools = await _mcpClient.ListToolsAsync();
        var options = new ChatOptions { Tools = tools.Cast<AITool>().ToList() };

        var response = await _chatClient.GetResponseAsync(messages, options);
        
        messages.Add(new ChatMessage(ChatRole.Assistant, response.Text ?? ""));
        await _historyService.SaveHistoryAsync(userId, messages);

        return response.Text ?? "Sem resposta";
    }
}