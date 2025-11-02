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
        private readonly string _apiKey; // MUDANÇA: Boa prática guardar a key aqui

        public NoticiasService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration["GNews:ApiKey"]; // MUDANÇA: Pegar a key uma vez no construtor
        }

        // MUDANÇA 1: Assinatura do método alterada para aceitar 'query' e 'page'
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

            // MUDANÇA 2: Lógica para o termo de busca padrão
            // Se a query do front-end for vazia, pesquisamos "finanças"
            var termoDeBuscaFinal = string.IsNullOrWhiteSpace(query) ? "finanças" : query;
            var termoCodificado = WebUtility.UrlEncode(termoDeBuscaFinal);
            
            // MUDANÇA 3: A URL agora inclui os parâmetros 'page' e 'max=10'
            // (A GNews usa 'page' para paginação e 'max' para o total de itens por página)
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

            // O seu código de padronização de JSON está excelente e permanece o mesmo.
            // Apenas garanti que a extração de 'source' está correta e segura.
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

        // MUDANÇA 4: ADIÇÃO DO NOVO MÉTODO PARA SUGESTÕES
        public async Task<List<string>> BuscarSugestoesAsync(string query)
        {
            var sugestoes = new List<string>();
            
            // Validação (não buscar sugestões se a API Key faltar)
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(query))
            {
                return sugestoes; // Retorna lista vazia
            }

            var termoCodificado = WebUtility.UrlEncode(query);
            
            // Estratégia: Usamos o 'search' mas pedimos só 5 resultados (max=5)
            // e só nos importamos com os títulos.
            var url = $"https://gnews.io/api/v4/search?q={termoCodificado}&lang=pt&max=5&token={_apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "SaveMoneyApp/1.0");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Falhar silenciosamente. Não queremos quebrar o app por causa de sugestões.
                Console.WriteLine($"Erro na API de sugestões: {response.StatusCode}");
                return sugestoes; // Retorna lista vazia
            }

            var responseJsonString = await response.Content.ReadAsStringAsync();

            try
            {
                // Parseamos o JSON e extraímos *apenas* os títulos
                using var doc = JsonDocument.Parse(responseJsonString);
                var root = doc.RootElement;

                if (root.TryGetProperty("articles", out var articlesElement) && articlesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var article in articlesElement.EnumerateArray())
                    {
                        // Adiciona o título (ou "Sem título") à nossa lista de strings
                        sugestoes.Add(article.GetPropertyOrDefault("title", null) ?? "Sem título");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao parsear sugestões: {ex.Message}");
                // Retorna lista vazia em caso de erro de parsing
            }

            return sugestoes;
        }
    }

    // MUDANÇA 5: ADIÇÃO DE UM MÉTODO DE EXTENSÃO (HELPER)
    // Coloque isso fora da classe 'NoticiasService', mas dentro do 'namespace'.
    // Isso limpa o código e previne erros ao tentar ler propriedades JSON que não existem.
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
                    // Adicione mais conversões de tipo se necessário
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