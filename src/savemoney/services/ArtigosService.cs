using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Collections.Generic;
using savemoney.Service; // Verifique se este namespace está correto ou se deve ser savemoney.Services
using System.Net; // Importante para o UrlEncode

namespace savemoney.Services
{
    public class ArtigosService : IConteudoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public ArtigosService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> BuscarConteudoAsync()
        {
            // --- INÍCIO DA SUA TAREFA ---

            // 1. Defina o seu termo de pesquisa.
            // O WebUtility.UrlEncode é crucial para tratar espaços e caracteres especiais!
            var termoDeBusca = WebUtility.UrlEncode("finanças pessoais");

            // 2. Defina os campos que você quer que a API retorne.
            var campos = "display_name,publication_year,authorships,primary_location";
            
            // 3. Monte a sua URL final para a OpenAlex API.
            var url = $"https://api.openalex.org/works?search={termoDeBusca}&select={campos}";

            // 4. Faça o pedido HTTP (o seu código aqui já está quase perfeito).
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            // O User-Agent não é estritamente necessário para a OpenAlex, mas é uma boa prática.
            request.Headers.Add("User-Agent", "SaveMoneyApp/1.0");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJsonString = await response.Content.ReadAsStringAsync();

            // 5. Por agora, vamos simplesmente retornar o JSON que recebemos.
            // O passo de padronizar o JSON pode vir depois.
            return responseJsonString;
            
            // --- FIM DA SUA TAREFA ---
        }
    }
}