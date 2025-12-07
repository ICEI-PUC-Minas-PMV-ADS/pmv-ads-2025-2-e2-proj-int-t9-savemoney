using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("Widget")]
    public class Widget
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
        public required string Titulo { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string? Descricao { get; set; }

        [StringLength(2000, ErrorMessage = "A URL da imagem deve ter no máximo 2000 caracteres.")]
        [Url(ErrorMessage = "Por favor, insira uma URL válida.")]
        public string? ImagemUrl { get; set; }

        [StringLength(200, ErrorMessage = "O link deve ter no máximo 200 caracteres.")]
        public string? Link { get; set; }

        [Range(1, 3, ErrorMessage = "Colunas deve ser entre 1 e 3.")]
        public int Colunas { get; set; } = 1;

        [Range(1, 3, ErrorMessage = "Largura deve ser entre 1 e 3.")]
        public int Largura { get; set; } = 1;

        public int PosicaoX { get; set; } = 0;

        public int PosicaoY { get; set; } = 0;
        // ADICIONE DEPOIS DA LINHA public int PosicaoY { get; set; } = 0;

        [Range(0, 100)]
        public int ZIndex { get; set; } = 0;

        public bool IsPinned { get; set; } = false;

        public bool IsVisivel { get; set; } = true;

        public DateTime? UltimaMovimentacao { get; set; }

        [StringLength(20)]
        public string? TipoWidget { get; set; } = "custom";

        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$",
            ErrorMessage = "Cor deve estar no formato hexadecimal (#RRGGBB).")]
        [StringLength(7)]
        public string CorFundo { get; set; } = "#1b1d29";

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relacionamento
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}