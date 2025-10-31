using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cibra.AgriculturalPosts.Domain.Entities;
using Cibra.AgriculturalPosts.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Cibra.AgriculturalPosts.Infrastructure.Services;

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public string Model { get; set; } = "gpt-4o-mini";
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
}

public class OpenAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAISettings _settings;
    private readonly AsyncRetryPolicy _retryPolicy;

    public OpenAIService(HttpClient httpClient, IOptions<OpenAISettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

        // Retry policy for rate limiting
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timeSpan.Seconds}s due to: {exception.Message}");
                }
            );
    }

    public async Task<PostAnalysis> AnalyzePostAsync(string content, CancellationToken cancellationToken = default)
    {
        var prompt = BuildAnalysisPrompt(content);

        var response = await _retryPolicy.ExecuteAsync(async () =>
            await CallOpenAIAsync(prompt, cancellationToken)
        );

        return ParseAnalysisResponse(response);
    }

    public async Task<string> ProcessUserMentionAsync(
        string query,
        string postContent,
        List<AIInteraction> history,
        CancellationToken cancellationToken = default)
    {
        var prompt = BuildUserMentionPrompt(query, postContent, history);

        return await _retryPolicy.ExecuteAsync(async () =>
            await CallOpenAIAsync(prompt, cancellationToken)
        );
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var testPrompt = "Test connection. Respond with 'OK'.";
            var response = await CallOpenAIAsync(testPrompt, CancellationToken.None);
            return !string.IsNullOrEmpty(response);
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> CallOpenAIAsync(string prompt, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _settings.Model,
            messages = new[]
            {
                new { role = "system", content = GetSystemPrompt() },
                new { role = "user", content = prompt }
            },
            max_tokens = _settings.MaxTokens,
            temperature = _settings.Temperature
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_settings.Endpoint, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"OpenAI API error: {response.StatusCode} - {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

        return openAIResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty;
    }

    private static string GetSystemPrompt()
    {
        return @"Você é um assistente especializado em análise agrícola. 
Sua função é analisar postagens sobre agricultura e fornecer insights técnicos precisos.
Você deve identificar: tipo de cultura, estágio de cultivo, problemas potenciais e recomendações.
Sempre responda em português do Brasil e seja técnico mas acessível.";
    }

    private static string BuildAnalysisPrompt(string content)
    {
        return $@"Analise a seguinte postagem agrícola e retorne um JSON estruturado:

POSTAGEM:
{content}

Retorne APENAS um JSON válido no seguinte formato:
{{
  ""cultureType"": ""tipo da cultura identificada"",
  ""stage"": ""Planting|Growing|Flowering|Harvesting|PostHarvest|Unknown"",
  ""problems"": [
    {{
      ""type"": ""Pest|Disease|Weather|Soil|Nutrition|Water|None"",
      ""description"": ""descrição do problema"",
      ""severity"": ""Baixa|Média|Alta""
    }}
  ],
  ""recommendations"": [""recomendação 1"", ""recomendação 2""],
  ""confidenceScore"": 0.95
}}";
    }

    private static string BuildUserMentionPrompt(string query, string postContent, List<AIInteraction> history)
    {
        var historyContext = history.Count > 0
            ? $"\n\nCONTEXTO DE CONVERSAS ANTERIORES:\n{string.Join("\n", history.Select(h => $"Usuário: {h.UserQuery}\nAssistente: {h.AIResponse}"))}"
            : string.Empty;

        return $@"POSTAGEM ORIGINAL:
{postContent}
{historyContext}

PERGUNTA DO USUÁRIO:
{query}

Responda de forma clara e técnica, fornecendo informações úteis sobre a questão levantada.";
    }

    private static PostAnalysis ParseAnalysisResponse(string response)
    {
        try
        {
            // Remove markdown code blocks if present
            var cleanJson = response.Trim();
            if (cleanJson.StartsWith("```json"))
            {
                cleanJson = cleanJson.Substring(7);
            }
            if (cleanJson.StartsWith("```"))
            {
                cleanJson = cleanJson.Substring(3);
            }
            if (cleanJson.EndsWith("```"))
            {
                cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
            }
            cleanJson = cleanJson.Trim();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var parsed = JsonSerializer.Deserialize<AIAnalysisResponse>(cleanJson, options);

            return new PostAnalysis
            {
                CultureType = parsed?.CultureType ?? "Não identificado",
                Stage = ParseStage(parsed?.Stage),
                Problems = parsed?.Problems?.Select(p => new IdentifiedProblem
                {
                    Type = ParseProblemType(p.Type),
                    Description = p.Description ?? "",
                    Severity = p.Severity ?? "Desconhecida"
                }).ToList() ?? new List<IdentifiedProblem>(),
                Recommendations = parsed?.Recommendations ?? new List<string>(),
                ConfidenceScore = parsed?.ConfidenceScore ?? 0.5,
                AnalyzedAt = DateTime.UtcNow,
                RawAIResponse = response
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to parse AI response: {ex.Message}");

            return new PostAnalysis
            {
                CultureType = "Erro na análise",
                Stage = CultivationStage.Unknown,
                Problems = new List<IdentifiedProblem>(),
                Recommendations = new List<string> { "Não foi possível analisar a postagem automaticamente." },
                ConfidenceScore = 0.0,
                AnalyzedAt = DateTime.UtcNow,
                RawAIResponse = response
            };
        }
    }

    private static CultivationStage ParseStage(string? stage)
    {
        return stage?.ToLower() switch
        {
            "planting" => CultivationStage.Planting,
            "growing" => CultivationStage.Growing,
            "flowering" => CultivationStage.Flowering,
            "harvesting" => CultivationStage.Harvesting,
            "postharvest" => CultivationStage.PostHarvest,
            _ => CultivationStage.Unknown
        };
    }

    private static ProblemType ParseProblemType(string? type)
    {
        return type?.ToLower() switch
        {
            "pest" => ProblemType.Pest,
            "disease" => ProblemType.Disease,
            "weather" => ProblemType.Weather,
            "soil" => ProblemType.Soil,
            "nutrition" => ProblemType.Nutrition,
            "water" => ProblemType.Water,
            _ => ProblemType.None
        };
    }
}

// Response DTOs for OpenAI
internal class OpenAIResponse
{
    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }
}

internal class Choice
{
    [JsonPropertyName("message")]
    public Message? Message { get; set; }
}

internal class Message
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

internal class AIAnalysisResponse
{
    [JsonPropertyName("cultureType")]
    public string? CultureType { get; set; }

    [JsonPropertyName("stage")]
    public string? Stage { get; set; }

    [JsonPropertyName("problems")]
    public List<AIProblem>? Problems { get; set; }

    [JsonPropertyName("recommendations")]
    public List<string>? Recommendations { get; set; }

    [JsonPropertyName("confidenceScore")]
    public double ConfidenceScore { get; set; }
}

internal class AIProblem
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("severity")]
    public string? Severity { get; set; }
}