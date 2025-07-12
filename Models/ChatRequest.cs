namespace SemanticKernalNewsAPI.Models;

public class ChatRequest
{
    public string SessionId { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
}