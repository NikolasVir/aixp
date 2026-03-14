using AIXP.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<EmbeddingService>();
builder.Services.AddSingleton<DocumentService>();
builder.Services.AddSingleton<SearchService>();
builder.Services.AddHttpClient<GenerationService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();

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

    var document = await documentService.CreateDocumentAsync(file);

    return TypedResults.Created($"/document/{document.Id}", new
    {
        id = document.Id,
        name = document.Name,
        pageCount = document.PageCount,
        chunkCount = document.ChunkCount,
        embeddingCount = document.Embeddings.Count,
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

// GET Document Chunks
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

// POST search within a document
app.MapPost("/document/{id:guid}/search", async Task<IResult> (
    Guid id,
    SearchRequest request,
    DocumentService documentService,
    EmbeddingService embeddingService,
    SearchService searchService) =>
{
    var document = documentService.GetDocument(id);

    if (document == null)
        return Results.NotFound($"Document with ID {id} not found");

    if (string.IsNullOrWhiteSpace(request.Query))
        return Results.BadRequest("Query cannot be empty");

    var queryEmbedding = await embeddingService.EmbedAsync(request.Query);
    var results = searchService.Search(document, queryEmbedding, request.TopN);

    return Results.Ok(new
    {
        query = request.Query,
        results = results.Select(r => new
        {
            chunkNumber = r.ChunkNumber,
            score = Math.Round(r.Score, 4),
            text = r.Text
        })
    });
})
.WithName("SearchDocument");

// POST ask a question about a document
app.MapPost("/document/{id:guid}/ask", async Task<IResult> (
    Guid id,
    AskRequest request,
    DocumentService documentService,
    EmbeddingService embeddingService,
    SearchService searchService,
    GenerationService generationService) =>
{
    var document = documentService.GetDocument(id);

    if (document == null)
        return Results.NotFound($"Document with ID {id} not found");

    if (string.IsNullOrWhiteSpace(request.Question))
        return Results.BadRequest("Question cannot be empty");

    // Step 1: Embed the question
    var queryEmbedding = await embeddingService.EmbedAsync(request.Question);

    // Step 2: Find relevant chunks
    var searchResults = searchService.Search(document, queryEmbedding, request.TopN);

    // Step 3: Generate answer
    var contextChunks = searchResults.Select(r => r.Text).ToList();
    var answer = await generationService.AskAsync(request.Question, contextChunks);

    return Results.Ok(new
    {
        question = request.Question,
        answer,
        sourceChunks = searchResults.Select(r => new
        {
            chunkNumber = r.ChunkNumber,
            score = Math.Round(r.Score, 4)
        })
    });
})
.WithName("AskDocument");

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