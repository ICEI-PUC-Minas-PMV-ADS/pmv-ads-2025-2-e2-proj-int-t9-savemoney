namespace savemoney.Services;
// Esta classe é responsável por buscar as noticias
public class NoticiasService
{
    //HttpClient é a ferramenta do C# para fazer pedidos na net
    private readonly HttpClient _HttpClient;
    private readonly IConfiguration _configuration;

    // Este é o construtor. Ele recebe as "Ferramentas" de que precisa para trabalhar
    public NoticiasService(HttpClient httpClient, IConfiguration configuration)
    {
        _HttpClient = httpClient;
        _configuration = configuration;
    }

    // Este é o metodo princiapl
    public async Task<string> BuscarNoticias()
    {
        // 1- Lê a chave da API que está no appsettings.Development.json
        var apiKey = _configuration["NewsApi:ApiKey"];

        // 2- Monta a URL da api externa. Este exemplo, busca noticias de negocios no Brasil
        var url = $"https://newsapi.org/v2/top-headlines?country=br&category=business&apiKey={apiKey}";

        //3 -Adionar um cabeçalho obrigatorio para a NewsApi
        _HttpClient.DefaultRequestHeaders.Add("User-Agent", "SaveMoneyApp/1.0");

        // 4 - fazer o pedido GET e obter a resposta como uma string de texto JSON
        var responseJsonString = await _HttpClient.GetStringAsync(url);

        // 5- retornar a string JSON que recebemos
        return responseJsonString;
    }
}