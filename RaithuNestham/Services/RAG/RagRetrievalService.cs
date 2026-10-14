
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace RaithuNestham.Services.RAG;

public class RagRetrievalService
{
	private readonly KnowledgeDocumentService _documentService;
	private readonly EmbeddingService _embeddingService;
	private readonly ILogger<RagRetrievalService> _logger;

	private static readonly ConcurrentDictionary<
		string,
		Lazy<Task<List<double>>>> EmbeddingCache = new();

	public RagRetrievalService(
		KnowledgeDocumentService documentService,
		EmbeddingService embeddingService,
		ILogger<RagRetrievalService> logger)
	{
		_documentService = documentService;
		_embeddingService = embeddingService;
		_logger = logger;
	}

	// Returns relevant chunks for the existing AI chat.
	public async Task<List<string>> RetrieveRelevantChunksAsync(
		string question,
		int topK = 3)
	{
		var results = await RetrieveScoredChunksAsync(question, topK);

		return results
			.Select(result => result.Chunk)
			.ToList();
	}

	// Returns chunks with their cosine similarity scores.
	public async Task<List<(string Chunk, double Score)>>
		RetrieveScoredChunksAsync(
			string question,
			int topK = 3)
	{
		if (string.IsNullOrWhiteSpace(question))
		{
			throw new ArgumentException(
				"Question cannot be empty.",
				nameof(question));
		}

		var chunks = await _documentService.LoadChunksAsync();

		if (chunks.Count == 0)
		{
			_logger.LogWarning(
				"No knowledge document chunks were found.");

			return new List<(string Chunk, double Score)>();
		}

		var questionVector =
			await _embeddingService.CreateEmbeddingAsync(question);

		var results = new List<(string Chunk, double Score)>();

		foreach (var chunk in chunks)
		{
			var cachedEmbedding = EmbeddingCache.GetOrAdd(
				chunk,
				text => new Lazy<Task<List<double>>>(() =>
				{
					_logger.LogInformation(
						"Generating embedding for a new document chunk.");

					return _embeddingService.CreateEmbeddingAsync(text);
				}));

			List<double> chunkVector;

			try
			{
				chunkVector = await cachedEmbedding.Value;
			}
			catch
			{
				// Remove the failed entry so a future request can retry.
				var cacheEntry = new KeyValuePair<
					string,
					Lazy<Task<List<double>>>>(
						chunk,
						cachedEmbedding);

				((ICollection<KeyValuePair<
					string,
					Lazy<Task<List<double>>>>>)EmbeddingCache)
					.Remove(cacheEntry);

				throw;
			}

			var score = CosineSimilarity(
				questionVector,
				chunkVector);

			results.Add((chunk, score));
		}

		var orderedResults = results
			.OrderByDescending(result => result.Score)
			.Take(Math.Clamp(topK, 1, chunks.Count))
			.ToList();

		_logger.LogInformation(
			"Retrieved {Count} chunks from {Total} chunks.",
			orderedResults.Count,
			chunks.Count);

		return orderedResults;
	}

	// Returns only chunks meeting the chosen similarity threshold.
	public async Task<List<string>>
		RetrieveRelevantChunksWithThresholdAsync(
			string question,
			int topK = 3,
			double minimumScore = 0.68)
	{
		if (minimumScore < -1 || minimumScore > 1)
		{
			throw new ArgumentOutOfRangeException(
				nameof(minimumScore),
				"Minimum cosine similarity must be between -1 and 1.");
		}

		var results = await RetrieveScoredChunksAsync(question, 6);

		var filteredResults = results
			.Where(result => result.Score >= minimumScore)
			.Take(Math.Clamp(topK, 1, 6))
			.Select(result => result.Chunk)
			.ToList();

		_logger.LogInformation(
			"Threshold retrieval returned {Count} chunks with minimum score {Score}.",
			filteredResults.Count,
			minimumScore);

		return filteredResults;
	}

	private static double CosineSimilarity(
		List<double> vectorA,
		List<double> vectorB)
	{
		if (vectorA.Count != vectorB.Count || vectorA.Count == 0)
		{
			throw new ArgumentException(
				"Embedding vectors must have the same non-zero length.");
		}

		double dotProduct = 0;
		double magnitudeA = 0;
		double magnitudeB = 0;

		for (int i = 0; i < vectorA.Count; i++)
		{
			dotProduct += vectorA[i] * vectorB[i];
			magnitudeA += vectorA[i] * vectorA[i];
			magnitudeB += vectorB[i] * vectorB[i];
		}

		if (magnitudeA == 0 || magnitudeB == 0)
		{
			return 0;
		}

		return dotProduct /
			(Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
	}
}
