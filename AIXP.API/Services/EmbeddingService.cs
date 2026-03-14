using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace AIXP.API.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _ollamaUrl;
    private readonly string _model;

    public EmbeddingService(HttpClient httpClient, IOptions<OllamaSettings> options)
    {
        _httpClient = httpClient;
        _ollamaUrl = $"{options.Value.BaseUrl}/api/embeddings";
        _model = options.Value.EmbeddingModel;
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var payload = new { model = _model, prompt = text };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_ollamaUrl, content);
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
