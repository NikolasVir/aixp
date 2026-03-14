using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIXP.API.Services;

public class GenerationService
{
    private readonly HttpClient _httpClient;
    private const string OllamaUrl = "http://localhost:11434/api/chat";
    private const string Model = "llama3.2";

    public GenerationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AskAsync(string question, List<string> contextChunks)
    {
        var context = string.Join("\n\n", contextChunks);

        var messages = new[]
        {
            new
            {
                role = "system",
                content = "You are a helpful assistant. Answer questions using only the provided context. If the answer is not in the context, say so."
            },
            new
            {
                role = "user",
                content = $"Context:\n{context}\n\nQuestion: {question}"
            }
        };

        var payload = new { model = Model, messages, stream = false };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(OllamaUrl, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<OllamaChatResponse>(responseJson);

        return result!.message.content;
    }

    private record OllamaMessage(
        [property: JsonPropertyName("role")] string role,
        [property: JsonPropertyName("content")] string content
    );

    private record OllamaChatResponse(
        [property: JsonPropertyName("message")] OllamaMessage message
    );
}
