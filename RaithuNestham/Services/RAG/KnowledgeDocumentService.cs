
using System.Text.RegularExpressions;

namespace RaithuNestham.Services.RAG;

public class KnowledgeDocumentService
{
	private readonly string _documentPath;

	public KnowledgeDocumentService(
		IWebHostEnvironment environment)
	{
		_documentPath = Path.Combine(
			environment.ContentRootPath,
			"KnowledgeBase",
			"rice-cultivation.txt");
	}

	public async Task<List<string>> LoadChunksAsync(
		int chunkSize = 800,
		int overlap = 100)
	{
		if (chunkSize <= 0)
		{
			throw new ArgumentOutOfRangeException(
				nameof(chunkSize),
				"Chunk size must be greater than zero.");
		}

		if (overlap < 0 || overlap >= chunkSize)
		{
			throw new ArgumentOutOfRangeException(
				nameof(overlap),
				"Overlap must be non-negative and smaller than chunk size.");
		}

		if (!File.Exists(_documentPath))
		{
			throw new FileNotFoundException(
				"Rice cultivation knowledge document was not found.",
				_documentPath);
		}

		var text = await File.ReadAllTextAsync(_documentPath);

		// Normalize whitespace while preserving sentence punctuation.
		text = Regex.Replace(text, @"\s+", " ").Trim();

		var chunks = new List<string>();
		var start = 0;

		while (start < text.Length)
		{
			var end = Math.Min(start + chunkSize, text.Length);

			// Prefer ending at a sentence boundary.
			if (end < text.Length)
			{
				var searchStart = Math.Min(start + chunkSize / 2, end);
				var boundary = -1;

				for (var i = end - 1; i >= searchStart; i--)
				{
					if (text[i] == '.' ||
						text[i] == '!' ||
						text[i] == '?' ||
						text[i] == ';')
					{
						boundary = i + 1;
						break;
					}
				}

				// If no sentence boundary is available, end at a word boundary.
				if (boundary == -1)
				{
					var space = text.LastIndexOf(' ', end - 1, end - start);

					if (space > start)
					{
						boundary = space;
					}
				}

				if (boundary > start)
				{
					end = boundary;
				}
			}

			var chunk = text[start..end].Trim();

			if (!string.IsNullOrWhiteSpace(chunk))
			{
				chunks.Add(chunk);
			}

			if (end >= text.Length)
			{
				break;
			}

			// Keep overlap, but start at a word boundary.
			var nextStart = Math.Max(start + 1, end - overlap);

			while (nextStart < end && text[nextStart] != ' ')
			{
				nextStart++;
			}

			if (nextStart < end)
			{
				nextStart++;
			}

			// Safety check: always move forward.
			start = Math.Max(nextStart, start + 1);
		}

		return chunks;
	}
}
