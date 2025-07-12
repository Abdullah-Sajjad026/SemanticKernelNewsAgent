using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using SemanticKernalNewsAPI.Models;
using SemanticKernalNewsAPI.Services;

namespace SemanticKernalNewsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly Kernel _kernel;

    private readonly GeminiPromptExecutionSettings _promptExecutionSettings = new GeminiPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
    };

    private readonly SessionHistoryStore _sessionHistory;

    public ChatController(Kernel kernel, SessionHistoryStore sessionHistory)
    {
        _kernel = kernel;
        _sessionHistory = sessionHistory;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
            return BadRequest("SessionId is required");

        var chatService = _kernel.GetRequiredService<IChatCompletionService>();
        var history = _sessionHistory.GetChatHistory(request.SessionId);

        // Add system message only once per session
        if (history.Count == 0)
        {
            history.AddSystemMessage(
                @"You are a professional news assistant with access to current news feeds. Follow these rules:

            1. Category Handling:
               - Always ask for a category if none is provided
               - Suggest common categories: Technology, Business, Sports, Science, Health
               - If given an invalid category, suggest similar valid ones

            2. News Formatting:
               Format each news item EXACTLY like this:
               [Category] [Publication Date]
               • Headline: [Title]
               • Summary: [Description]
               • Read more: [URL]
               ━━━━━━━━━━━━━━━━━━━━━━━━━━━━

            3. Behavior:
               - Always return 5 items unless requested otherwise
               - Summarize complex topics in simple terms
               - Never invent news - only use actual feed content
               - If no news found, explain why and suggest alternatives

            Example response:
            'Here are the latest Technology news stories:
            Technology | July 12, 2025
            • Headline: Google Hires Top A.I. Leaders
            • Summary: Google recruited Windsurf executives in $2.4B deal...
            • Read more: https://nyt.com/link1
            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            [4 more items formatted the same way]'");
        }

        history.AddUserMessage(request.Message);

        var response = "";

        await foreach (var chunk in chatService.GetStreamingChatMessageContentsAsync(history, _promptExecutionSettings,
                           _kernel))
        {
            response += chunk;
            Console.WriteLine(chunk);
        }

        history.AddAssistantMessage(response);
        return Ok(new ChatResponse { Reply = response });
    }
}