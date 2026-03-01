using UglyToad.PdfPig;

public record Document
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTime TimeUploaded { get; init; }
    public long FileSizeBytes { get; init; }

    // PageTexts: a PageText is the whole text content of a page
    public List<string> PageTexts { get; set; }

    public int PageCount => PageTexts.Count;

    public Document(IFormFile pdfFile)
    {
        Id = Guid.NewGuid();
        Name = pdfFile.FileName;
        TimeUploaded = DateTime.UtcNow;
        FileSizeBytes = pdfFile.Length;
        PageTexts = ExtractTextPerPage(pdfFile);
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

}
