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

        public NoticiasService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> BuscarNoticiasAsync(string termoDeBusca)
        {
            var apiKey = _configuration["GNews:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
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

            // Adiciona o filtro de notícias financeiras (business)
            var termoCodificado = WebUtility.UrlEncode(termoDeBusca);
            var url = $"https://gnews.io/api/v4/top-headlines?lang=pt&topic=business&token={apiKey}";
            // var url = $"https://gnews.io/api/v4/search?q={termoCodificado}&lang=pt&token={apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "SaveMoneyApp/1.0");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJsonString = await response.Content.ReadAsStringAsync();

            // Padroniza o retorno
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
                            title = article.GetPropertyOrDefault("title", "Sem título"),
                            description = article.GetPropertyOrDefault("description", "Sem descrição"),
                            url = article.GetPropertyOrDefault("url", "#"),
                            urlToImage = article.GetPropertyOrDefault("image", null),
                            publishedAt = article.GetPropertyOrDefault("publishedAt", null),
                            source = article.TryGetProperty("source", out var source) && source.TryGetProperty("name", out var name) ? name.GetString() : null
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
                // Retorno padrão em caso de erro de parsing
                var erro = new
                {
                    status = "error",
                    totalResults = 0,
                    articles = new List<object>()
                };
                return JsonSerializer.Serialize(erro);
            }
        }
    }

    // Métodos de extensão para facilitar o tratamento de campos ausentes
    public static class JsonExtensions
    {
        public static string GetPropertyOrDefault(this JsonElement element, string property, string defaultValue)
        {
            if (element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String)
                return value.GetString();
            return defaultValue;
        }

        public static int GetPropertyOrDefault(this JsonElement element, string property, int defaultValue)
        {
            if (element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
                return intValue;
            return defaultValue;
        }
    }
}