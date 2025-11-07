using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System;
using savemoney.Models;
using System.Text;

namespace savemoney.Services
{
    public class ArtigosService
    {
        private readonly HttpClient _httpClient;
        private const string USER_AGENT = "SaveMoneyApp/1.0 (mailto:contato@savemoneyapp.com)";

        public ArtigosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> BuscarArtigosAsync(ArtigoBuscaRequest request)
        {
            try
            {
                var url = ConstruirUrlOpenAlex(request);

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Add("User-Agent", USER_AGENT);

                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {

                    var errorBody = await response.Content.ReadAsStringAsync();
                    return RetornarErroPadronizado($"Erro da API OpenAlex: {response.StatusCode}. Detalhes: {errorBody}");
                }

                var responseJsonString = await response.Content.ReadAsStringAsync();
                
                return ProcessarRespostaOpenAlex(responseJsonString, request.PageSize);
            }
            catch (Exception ex)
            {

                return RetornarErroPadronizado($"Erro interno no serviço (Timeout?): {ex.Message}");
            }
        }

        
        private string ConstruirUrlOpenAlex(ArtigoBuscaRequest request)
        {
            var baseUrl = "https://api.openalex.org/works";
            var filters = new StringBuilder();

            // --- 1. FILTRO DE TEXTO (SearchTerm) ---
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filters.Append($"title_and_abstract.search:{WebUtility.UrlEncode(request.SearchTerm)}");
            }
            else
            {

                filters.Append("default.search:finance"); 
            }
            
            // --- 2. FILTRO DE IDIOMA (Region) ---
            if (request.Region == "BR")
            {
                if (filters.Length > 0) filters.Append(',');
                filters.Append("language:pt");
            }
            
            // --- 3. FILTRO DE ORDEM (Sort) ---
            string sortOrder;
            if (request.SortOrder == "newest")
            {
                sortOrder = "publication_date:desc";
            }
            else
            {
                sortOrder = "cited_by_count:desc";
            }

            // --- 4. PAGINAÇÃO ---
            int page = request.Page > 0 ? request.Page : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 6;

            // --- Montagem final da URL ---
            return $"{baseUrl}?filter={filters.ToString()}&sort={sortOrder}&per-page={pageSize}&page={page}";
        }

        
        private string ProcessarRespostaOpenAlex(string jsonString, int pageSize)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;
                var articles = new List<object>();

                if (root.TryGetProperty("results", out var resultsElement) && resultsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var work in resultsElement.EnumerateArray())
                    {
                        var title = work.GetPropertyOrDefault("title", null) ?? "Sem título";
                        var url = work.GetPropertyOrDefault("doi", null) ?? work.GetPropertyOrDefault("id", null) ?? "#";
                        var publicationDate = work.GetPropertyOrDefault("publication_date", null);
                        var citedByCount = work.GetPropertyOrDefault("cited_by_count", 0);

                        string? abstractText = null;
                        if (work.TryGetProperty("abstract_inverted_index", out var abstractInvertedIndex) && abstractInvertedIndex.ValueKind == JsonValueKind.Object && abstractInvertedIndex.EnumerateObject().Any())
                        {
                            abstractText = ReconstruirAbstract(abstractInvertedIndex);
                        }

                        var authorsList = ExtrairAutores(work);
                        string sourceName = ExtrairFonte(work);

                        articles.Add(new
                        {
                            title,
                            url,
                            abstractText,
                            publicationDate,
                            authors = authorsList,
                            source = sourceName,
                            citedByCount
                        });
                    }
                }
                
                bool hasNextPage = articles.Count == pageSize;

                var padronizado = new
                {
                    status = "ok",
                    totalResults = root.TryGetProperty("meta", out var meta) ? meta.GetPropertyOrDefault("count", articles.Count) : articles.Count,
                    articles,
                    hasNextPage
                };

                return JsonSerializer.Serialize(padronizado);
            }
            catch (Exception ex)
            {
                return RetornarErroPadronizado($"Erro ao processar a resposta JSON: {ex.Message}");
            }
        }


        private static string ReconstruirAbstract(JsonElement invertedIndex)
        {
            var wordPositions = new SortedDictionary<int, string>();
            foreach (var property in invertedIndex.EnumerateObject())
            {
                var word = property.Name;
                var positions = property.Value;
                if (positions.ValueKind == JsonValueKind.Array)
                {
                    foreach (var position in positions.EnumerateArray())
                    {
                        if (position.TryGetInt32(out int pos) && !wordPositions.ContainsKey(pos))
                        {
                            wordPositions.Add(pos, word);
                        }
                    }
                }
            }
            return string.Join(" ", wordPositions.Values);
        }

        private List<string> ExtrairAutores(JsonElement work)
        {
            var authors = new List<string>();
            if (work.TryGetProperty("authorships", out var authorshipsElement) && authorshipsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var authorship in authorshipsElement.EnumerateArray())
                {
                    if (authorship.TryGetProperty("author", out var authorElement) && authorElement.TryGetProperty("display_name", out var nameElement))
                    {
                        var name = nameElement.GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            authors.Add(name);
                        }
                    }
                }
            }
            return authors;
        }

        private string ExtrairFonte(JsonElement work)
        {
            if (work.TryGetProperty("primary_location", out var primaryLocation) && primaryLocation.ValueKind != JsonValueKind.Null &&
                primaryLocation.TryGetProperty("source", out var source) && source.ValueKind != JsonValueKind.Null &&
                source.TryGetProperty("display_name", out var sourceDisplayName))
            {
                return sourceDisplayName.GetString() ?? "Fonte desconhecida";
            }
            return "Fonte desconhecida";
        }

        private string RetornarErroPadronizado(string mensagem)
        {
            var erro = new
            {
                status = "error",
                message = mensagem,
                totalResults = 0,
                articles = new List<object>()
            };
            return JsonSerializer.Serialize(erro);
        }
    }
}