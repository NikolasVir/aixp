namespace AIXP.API.Services;

public class DocumentService
{
    private readonly Dictionary<Guid, Document> _documents = new();

    public Document CreateDocument(IFormFile file)
    {
        var document = new Document(file);
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
