using UglyToad.PdfPig;

public record Document
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTime TimeUploaded { get; init; }
    public long FileSizeBytes { get; init; }

    // PageTexts: a PageText is the whole text content of a page
    public List<string> PageTexts { get; set; }

    // Chunks: a Chunk is a portion of text
    public List<string> Chunks { get; set; }

    // Parallel to Chunks - Embeddings[i] is the embedding vector for Chunks[i]
    public List<float[]> Embeddings { get; set; } = new();

    public int PageCount => PageTexts.Count;
    public int ChunkCount => Chunks.Count;

    public Document(IFormFile pdfFile)
    {
        Id = Guid.NewGuid();
        Name = pdfFile.FileName;
        TimeUploaded = DateTime.UtcNow;
        FileSizeBytes = pdfFile.Length;
        PageTexts = ExtractTextPerPage(pdfFile);
        Chunks = ChunkPageTexts(PageTexts);
    }

    private List<string> ExtractTextPerPage(IFormFile file)
    {
        var pages = new List<string>();

        using var stream = file.OpenReadStream();
        using var pdf = UglyToad.PdfPig.PdfDocument.Open(stream);

        foreach (var page in pdf.GetPages())
        {
            var text = string.Join(" ", page.GetWords().Select(w => w.Text));
            pages.Add(text);
        }

        return pages;
    }

    private List<string> ChunkPageTexts(List<string> pageTexts)
    {
        var words = pageTexts
            .SelectMany(page => page.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToArray();

        const int chunkSize = 250;
        const int overlap = 50;
        const int step = chunkSize - overlap;

        var chunks = new List<string>();

        for (int i = 0; i < words.Length; i += step)
        {
            var chunkWords = words.Skip(i).Take(chunkSize);
            chunks.Add(string.Join(" ", chunkWords));
        }

        return chunks;
    }

}
