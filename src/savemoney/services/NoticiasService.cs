using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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

        public async Task<string> BuscarNoticias()
        {
            var apiKey = _configuration["NewsApi:ApiKey"];

            // --- CORREÇÃO APLICADA AQUI ---
            // Definimos as fontes de notícias em que confiamos.
            var fontes = "info-money,google-news-br,globo";

            // Mudamos a URL para usar o endpoint 'top-headlines' com o parâmetro 'sources'.
            // Isto é muito mais preciso do que pesquisar por uma palavra-chave em todo o lado.
            var url = $"https://newsapi.org/v2/top-headlines?sources={fontes}&apiKey={apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "SaveMoneyApp/1.0");

            var response = await _httpClient.SendAsync(request);

            // Garante que o pedido foi bem-sucedido
            response.EnsureSuccessStatusCode();

            var responseJsonString = await response.Content.ReadAsStringAsync();

            return responseJsonString;
        }
    }
}