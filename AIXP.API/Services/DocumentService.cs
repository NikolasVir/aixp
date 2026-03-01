namespace AIXP.API.Services;

public class DocumentService
{
    private readonly Dictionary<Guid, Document> _documents = new();
    private readonly EmbeddingService _embeddingService;

    public DocumentService(EmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public async Task<Document> CreateDocumentAsync(IFormFile file)
    {
        var document = new Document(file);
        document.Embeddings = await _embeddingService.EmbedAllAsync(document.Chunks);
        _documents.Add(document.Id, document);
        return document;
    }

    public Document? GetDocument(Guid id)
    {
        return _documents.GetValueOrDefault(id);
    }

    public void RemoveDocument(Guid id)
    {
        _documents.Remove(id);
    }

    public IEnumerable<Document> GetAllDocuments()
    {
        return _documents.Values;
    }
}
