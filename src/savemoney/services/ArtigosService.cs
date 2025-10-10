using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Collections.Generic;
using savemoney.Service;

namespace savemoney.Services
{
    public class ArtigosService : IConteudoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
    }