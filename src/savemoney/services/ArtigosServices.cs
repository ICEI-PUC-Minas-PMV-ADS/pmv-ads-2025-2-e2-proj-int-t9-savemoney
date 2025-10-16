using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System;

namespace savemoney.Services
{
    // Certifique-se que o nome da classe é ArtigosService
    public class ArtigosService
    {
        private readonly HttpClient _httpClient;
        private const string USER_AGENT = "SaveMoneyApp/1.0 (mailto:contato@savemoneyapp.com)";

        public ArtigosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ATUALIZADO: Adicionado '?' pois o termo pode vir nulo do Controller
        public async Task<string> BuscarArtigosAsync(string? termoDeBusca)
        {
            // 1. Definir Termo Padrão (se necessário)
            if (string.IsNullOrWhiteSpace(termoDeBusca))
            {
                termoDeBusca = "finance OR investment OR \"financial education\" OR economics";
            }

            // termoDeBusca não é mais nulo aqui.
            var termoCodificado = WebUtility.UrlEncode(termoDeBusca);

            // 2. Construir URL
            var url = $"https://api.openalex.org/works?search={termoCodificado}&sort=cited_by_count:desc&per-page=25";

            // 3. Fazer a Requisição
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", USER_AGENT);

            var response = await _httpClient.SendAsync(request);

            // 4. Tratamento de Erros da API Externa
            if (!response.IsSuccessStatusCode)
            {
                return RetornarErroPadronizado($"Erro ao comunicar com a API OpenAlex. Status: {response.StatusCode}");
            }

            var responseJsonString = await response.Content.ReadAsStringAsync();

            // 5. Processar e Padronizar a Resposta
            return ProcessarRespostaOpenAlex(responseJsonString);
        }

        private string ProcessarRespostaOpenAlex(string jsonString)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                var articles = new List<object>();

                // Os resultados estão na propriedade 'results'
                if (root.TryGetProperty("results", out var resultsElement) && resultsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var work in resultsElement.EnumerateArray())
                    {
                        // Extração de Dados Básicos
                        // Usamos "??" para garantir um valor padrão mesmo se a API retornar explicitamente "title": null
                        var title = work.GetPropertyOrDefault("title", null) ?? "Sem título";

                        // ATUALIZADO: GetPropertyOrDefault agora aceita null como padrão.
                        // Usamos o operador '??' (null-coalescing) para tentar o ID se o DOI for nulo.
                        // Usamos GetPropertyOrDefault para o ID também, que é mais seguro.
                        // Garantimos um fallback final para "#" caso ambos falhem.
                        var url = work.GetPropertyOrDefault("doi", null) ?? work.GetPropertyOrDefault("id", null) ?? "#";
                        
                        // A data pode ser nula.
                        var publicationDate = work.GetPropertyOrDefault("publication_date", null);
                        var citedByCount = work.GetPropertyOrDefault("cited_by_count", 0);

                        // ATUALIZADO: Declarado como string? pois pode ser nulo.
                        string? abstractText = null;
                        if (work.TryGetProperty("abstract_inverted_index", out var abstractInvertedIndex) && abstractInvertedIndex.ValueKind == JsonValueKind.Object && abstractInvertedIndex.EnumerateObject().Any())
                        {
                            abstractText = ReconstruirAbstract(abstractInvertedIndex);
                        }

                        // Extração de Autores e Fonte
                        var authorsList = ExtrairAutores(work);
                        string sourceName = ExtrairFonte(work);

                        // Padronização do objeto para o frontend
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

                // Retorno de sucesso padronizado
                var padronizado = new
                {
                    status = "ok",
                    totalResults = root.TryGetProperty("meta", out var meta) ? meta.GetPropertyOrDefault("count", articles.Count) : articles.Count,
                    articles
                };

                return JsonSerializer.Serialize(padronizado);

            }
            catch (Exception ex)
            {
                return RetornarErroPadronizado($"Erro ao processar a resposta JSON: {ex.Message}");
            }
        }

        // --- Métodos Auxiliares ---

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
                        // ATUALIZADO: GetString() pode retornar nulo, verificamos antes de adicionar.
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
                // ATUALIZADO: GetString() pode retornar nulo. Usamos '??' para garantir um valor padrão.
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