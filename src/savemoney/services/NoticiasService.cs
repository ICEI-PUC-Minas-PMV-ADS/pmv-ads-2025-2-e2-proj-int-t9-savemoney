using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;

namespace savemoney.Services
{
    public class NoticiasService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey; 

        public NoticiasService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration["GNews:ApiKey"];
        }

        public async Task<string> BuscarNoticiasAsync(string query, int page)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                var erro = new
                {
                    status = "error",
                    message = "API key is missing.",
                    totalResults = 0,
                    articles = new List<object>()
                };
                return JsonSerializer.Serialize(erro);
            }

            var termoDeBuscaFinal = string.IsNullOrWhiteSpace(query) ? "finanças" : query;
            var termoCodificado = WebUtility.UrlEncode(termoDeBuscaFinal);
            
            var url = $"https://gnews.io/api/v4/search?q={termoCodificado}&lang=pt&page={page}&max=10&token={_apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "SaveMoneyApp/1.0");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var erro = new
                {
                    status = "error",
                    message = $"Erro ao comunicar com a API de notícias. Status: {response.StatusCode}",
                    totalResults = 0,
                    articles = new List<object>()
                };
                return JsonSerializer.Serialize(erro);
            }

            var responseJsonString = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseJsonString);
                var root = doc.RootElement;

                var articles = new List<object>();
                if (root.TryGetProperty("articles", out var articlesElement) && articlesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var article in articlesElement.EnumerateArray())
                    {
                        articles.Add(new
                        {
                            title = article.GetPropertyOrDefault("title", null) ?? "Sem título",
                            description = article.GetPropertyOrDefault("description", null) ?? "Sem descrição",
                            url = article.GetPropertyOrDefault("url", null) ?? "#",
                            urlToImage = article.GetPropertyOrDefault("image", null),
                            publishedAt = article.GetPropertyOrDefault("publishedAt", null),
                            source = article.TryGetProperty("source", out var sourceEl) && sourceEl.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null
                        });
                    }
                }

                var padronizado = new
                {
                    status = "ok",
                    totalResults = root.GetPropertyOrDefault("totalArticles", 0),
                    articles
                };

                return JsonSerializer.Serialize(padronizado);
            }
            catch
            {
               
                var erro = new
                {
                    status = "error",
                    totalResults = 0,
                    articles = new List<object>()
                };
                return JsonSerializer.Serialize(erro);
            }
        }

        public async Task<List<string>> BuscarSugestoesAsync(string query)
        {
            var sugestoes = new List<string>();
            
            
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(query))
            {
                return sugestoes; 
            }

            var termoCodificado = WebUtility.UrlEncode(query);
            
            
            var url = $"https://gnews.io/api/v4/search?q={termoCodificado}&lang=pt&max=5&token={_apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "SaveMoneyApp/1.0");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                
                Console.WriteLine($"Erro na API de sugestões: {response.StatusCode}");
                return sugestoes; 
            }

            var responseJsonString = await response.Content.ReadAsStringAsync();

            try
            {

                using var doc = JsonDocument.Parse(responseJsonString);
                var root = doc.RootElement;

                if (root.TryGetProperty("articles", out var articlesElement) && articlesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var article in articlesElement.EnumerateArray())
                    {

                        sugestoes.Add(article.GetPropertyOrDefault("title", null) ?? "Sem título");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao parsear sugestões: {ex.Message}");

            }

            return sugestoes;
        }
    }

    public static class JsonElementExtensions
    {
        public static T GetPropertyOrDefault<T>(this JsonElement element, string propertyName, T defaultValue = default)
        {
            try
            {
                if (element.TryGetProperty(propertyName, out var property))
                {
                    if (typeof(T) == typeof(string) && property.ValueKind == JsonValueKind.String)
                    {
                        return (T)(object)property.GetString();
                    }
                    if (typeof(T) == typeof(int) && property.ValueKind == JsonValueKind.Number)
                    {
                        return (T)(object)property.GetInt32();
                    }
                    if (property.ValueKind == JsonValueKind.Null)
                    {
                        return defaultValue;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler propriedade JSON '{propertyName}': {ex.Message}");
            }
            return defaultValue;
        }
    }
}