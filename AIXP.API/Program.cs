using AIXP.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<DocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Create a document
app.MapPost("/document", async Task<IResult> (HttpContext context, DocumentService documentService) =>
{
    var form = await context.Request.ReadFormAsync();
    var file = form.Files["file"];

    if (file == null || file.Length == 0)
        return TypedResults.BadRequest("No file uploaded");

    if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        return TypedResults.BadRequest("Only PDF files allowed");

    // Use service to create document
    var document = documentService.CreateDocument(file);

    return TypedResults.Created($"/document/{document.Id}", new
    {
        id = document.Id,
        name = document.Name,
        pageCount = document.PageCount,
        uploadedAt = document.TimeUploaded
    });
})
.WithName("CreateDocument");

// GET single document by ID
app.MapGet("/document/{id:guid}", (Guid id, DocumentService documentService) =>
{
    var document = documentService.GetDocument(id);

    if (document == null)
        return Results.NotFound($"Document with ID {id} not found");

    return Results.Ok(new
    {
        id = document.Id,
        name = document.Name,
        pageCount = document.PageCount,
        fileSizeBytes = document.FileSizeBytes,
        uploadedAt = document.TimeUploaded
    });
})
.WithName("GetDocumentById");

// GET all documents
app.MapGet("/documents", (DocumentService documentService) =>
{
    var documents = documentService.GetAllDocuments();

    return Results.Ok(documents.Select(d => new
    {
        id = d.Id,
        name = d.Name,
        pageCount = d.PageCount,
        fileSizeBytes = d.FileSizeBytes,
        uploadedAt = d.TimeUploaded
    }));
})
.WithName("GetAllDocuments");

// GET page texts for a document
app.MapGet("/document/{id:guid}/pages", (Guid id, DocumentService documentService) =>
{
    var document = documentService.GetDocument(id);

    if (document == null)
        return Results.NotFound($"Document with ID {id} not found");

    return Results.Ok(new
    {
        id = document.Id,
        name = document.Name,
        pageCount = document.PageCount,
        pages = document.PageTexts.Select((text, index) => new
        {
            pageNumber = index + 1,
            text
        })
    });
})
.WithName("GetDocumentPageTextsById");

app.MapGet("/document/{id:guid}/chunks", (Guid id, DocumentService documentService) =>
{
    var document = documentService.GetDocument(id);

    if (document == null)
        return Results.NotFound($"Document with ID {id} not found");

    return Results.Ok(new
    {
        id = document.Id,
        name = document.Name,
        chunkCount = document.ChunkCount,
        chunks = document.Chunks.Select((text, index) => new
        {
            chunkNumber = index + 1,
            wordCount = text.Split(' ').Length,
            text
        })
    });
})
.WithName("GetDocumentChunksById");

// DELETE document by ID
app.MapDelete("/document/{id:guid}", (Guid id, DocumentService documentService) =>
{
    var document = documentService.GetDocument(id);

    if (document == null)
        return Results.NotFound($"Document with ID {id} not found");

    documentService.RemoveDocument(id);

    return Results.NoContent();  // 204 No Content (success, no body)
})
.WithName("DeleteDocumentById");


app.Run();