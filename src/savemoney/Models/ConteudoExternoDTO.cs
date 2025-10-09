namespace savemoney.Models;
/* Esta classe é um Data Transfer Object (DTO)
A única função é transportar dados de um lugar para o outro*/
public class ConteudoExternoDTO
{
    public string? Titulo { get; set; }
    public string? Resumo { get; set; }
    public string? Url { get; set; }
    public string? UrlDaImagem { get; set; }
    public string? Fonte { get; set; }
}