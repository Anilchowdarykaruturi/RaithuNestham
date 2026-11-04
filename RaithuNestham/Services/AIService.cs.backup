
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Services.Interfaces;
using RaithuNestham.Services.RAG;

namespace RaithuNestham.Services;

public class AIService : IAIService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly RagRetrievalService _ragRetrievalService;

    public AIService(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        RagRetrievalService ragRetrievalService)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
        _ragRetrievalService = ragRetrievalService;
    }

    public async Task<string> AskAsync(
        int farmerId,
        string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return "Please enter a question.";
        }

        var farmer = await _context.Farmers
            .Include(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == farmerId);

        if (farmer == null)
        {
            return "Farmer information was not found.";
        }

        var apiKey =
            Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "GEMINI_API_KEY environment variable is not configured.");
        }

        // STEP 1: Retrieve relevant farming information using RAG.
        var relevantChunks =
            await _ragRetrievalService.RetrieveRelevantChunksAsync(
                question,
                3);

        var retrievedContext = relevantChunks.Count > 0
            ? string.Join(
                "\n\n--- Retrieved document section ---\n\n",
                relevantChunks)
            : "No relevant information was found in the farming knowledge documents.";

        // STEP 2: Add the retrieved information to the Gemini prompt.
        var prompt =
            "You are RaithuNestham, an AI farming assistant for farmers in Andhra Pradesh. " +
            "Give clear, practical, safe and easy-to-understand farming guidance. " +
            "Consider the farmer's information when relevant.\n\n" +

            "RETRIEVED FARMING KNOWLEDGE:\n" +
            retrievedContext +
            "\n\n" +

            "RAG GROUNDING RULES:\n" +
            "Use the retrieved farming knowledge as reference material when relevant. " +
            "Answer using the retrieved information when it supports the question. " +
            "Do not claim that the documents contain information they do not contain. " +
            "If the retrieved information is insufficient, clearly say so and ask for " +
            "additional details or recommend checking reliable local agricultural guidance. " +
            "You may provide general knowledge when useful, but distinguish it from " +
            "information found in the retrieved documents. " +
            "Never invent facts or pretend that a source was consulted when it was not.\n\n" +

            "FARMING SAFETY RULES:\n" +
            "Treat fertilizer, pesticide, herbicide, fungicide, and crop-treatment quantities " +
            "as general guidance only, not as a fixed prescription. " +
            "Recommendations should consider the crop, crop stage, soil condition, soil-test results, " +
            "crop variety, season, irrigation, and local agricultural recommendations when relevant. " +
            "Encourage soil testing before making fertilizer recommendations when appropriate. " +
            "For chemical products, advise farmers to follow the product label and applicable local " +
            "agricultural guidance. " +
            "Do not invent product labels, government schemes, dosages, prices, or official recommendations. " +
            "If important information is missing, clearly state the assumption or ask a short follow-up question. " +
            "When giving quantities, clearly state that they are general guidance and may need adjustment.\n\n" +

            "LANGUAGE RULE - THIS IS MANDATORY:\n" +
            "You MUST answer every question in BOTH Telugu and English. " +
            "The Telugu section MUST use Telugu script. " +
            "Do NOT write Telugu using English letters. " +
            "Always provide Telugu first and English second. " +
            "Never provide only English.\n\n" +

            "REQUIRED RESPONSE FORMAT:\n\n" +
            "తెలుగు:\n" +
            "[Write the complete answer in Telugu script]\n\n" +
            "English:\n" +
            "[Write the complete English translation]\n\n" +

            $"Farmer name: {farmer.FullName}\n" +
            $"Question: {question}";

        // STEP 3: Send the question and retrieved context to Gemini.
        var requestBody = new
        {
            model = "gemini-3.6-flash",
            input = prompt
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://generativelanguage.googleapis.com/v1beta/interactions");

        request.Headers.Add("x-goog-api-key", apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response =
            await _httpClient.SendAsync(request);

        var responseContent =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Gemini API error: {response.StatusCode} - {responseContent}");
        }

        using var document =
            JsonDocument.Parse(responseContent);

        if (document.RootElement.TryGetProperty(
            "output",
            out var output))
        {
            return output.ToString();
        }

        if (document.RootElement.TryGetProperty(
            "steps",
            out var steps))
        {
            foreach (var step in steps.EnumerateArray())
            {
                if (step.TryGetProperty(
                    "type",
                    out var type) &&
                    type.GetString() == "model_output")
                {
                    if (step.TryGetProperty(
                        "content",
                        out var content))
                    {
                        foreach (var item in content.EnumerateArray())
                        {
                            if (item.TryGetProperty(
                                "text",
                                out var text))
                            {
                                return text.GetString()
                                    ?? "No response received from Gemini.";
                            }
                        }
                    }
                }
            }
        }

        return "No response received from Gemini.";
    }
}
