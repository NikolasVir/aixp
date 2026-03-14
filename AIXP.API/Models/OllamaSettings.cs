namespace AIXP.API.Services;

public class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3.2";
    public string EmbeddingModel { get; set; } = "bge-m3";
}
