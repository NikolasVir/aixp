public record Document
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTime TimeUploaded { get; init; }
    public long FileSizeBytes { get; init; }
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
        // TODO: Add PDF library (iTextSharp, PDFium, etc.)
        // For now, placeholder:
        var pages = new List<string>();
        
        using var stream = file.OpenReadStream();
        // Extract logic will go here once we add PDF library
        
        return pages;
    }
}
