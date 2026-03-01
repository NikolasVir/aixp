using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIXP.API.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;
    private const string OllamaUrl = "http://localhost:11434/api/embeddings";
    private const string Model = "bge-m3";

    public EmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var payload = new { model = Model, prompt = text };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(OllamaUrl, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseJson);

        return result!.embedding;
    }

    public async Task<List<float[]>> EmbedAllAsync(List<string> texts)
    {
        var embeddings = new List<float[]>();

        foreach (var text in texts)
        {
            var embedding = await EmbedAsync(text);
            embeddings.Add(embedding);
        }

        return embeddings;
    }

    private record OllamaEmbeddingResponse(
        [property: JsonPropertyName("embedding")] float[] embedding
    );

}
