using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("UserTheme")]
    public class UserTheme
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O nome do tema é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public required string NomeTema { get; set; }

        [Required]
        [StringLength(7)]
        public string BgPrimary { get; set; } = "#10111a";

        [Required]
        [StringLength(7)]
        public string BgSecondary { get; set; } = "#0d0e16";

        [Required]
        [StringLength(7)]
        public string BgCard { get; set; } = "#1b1d29";

        [Required]
        [StringLength(7)]
        public string BorderColor { get; set; } = "#2a2c3c";

        [Required]
        [StringLength(7)]
        public string TextPrimary { get; set; } = "#f5f5ff";

        [Required]
        [StringLength(7)]
        public string TextSecondary { get; set; } = "#aaaaaa";

        [Required]
        [StringLength(7)]
        public string AccentPrimary { get; set; } = "#3b82f6";

        [Required]
        [StringLength(7)]
        public string AccentPrimaryHover { get; set; } = "#2563eb";

        [Required]
        [StringLength(7)]
        public string BtnPrimaryText { get; set; } = "#ffffff";

        public bool IsAtivo { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relacionamento
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}