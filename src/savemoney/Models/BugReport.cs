using System;

namespace savemoney.Models
{
    public class BugReport
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Pagina { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Gravidade { get; set; } = "media"; // baixa, media, alta, critica
        public string UserAgent { get; set; } = string.Empty;
        public string? UsuarioNome { get; set; }
        public int? UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public bool Resolvido { get; set; } = false;
    }
}