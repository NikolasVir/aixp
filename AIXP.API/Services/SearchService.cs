namespace AIXP.API.Services;

public class SearchService
{
    public List<SearchResult> Search(Document document, float[] queryEmbedding, int topN = 5)
    {
        return document.Chunks
            .Select((chunk, index) => new SearchResult(
                ChunkNumber: index + 1,
                Text: chunk,
                Score: CosineSimilarity(queryEmbedding, document.Embeddings[index])
            ))
            .OrderByDescending(r => r.Score)
            .Take(topN)
            .ToList();
    }

    private float CosineSimilarity(float[] a, float[] b)
    {
        float dot = 0, magA = 0, magB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }

        return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
    }
}

public record SearchResult(int ChunkNumber, string Text, float Score);
