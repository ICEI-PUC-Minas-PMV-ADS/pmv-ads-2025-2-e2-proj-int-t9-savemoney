using savemoney.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "O documento é obrigatório.")]
        public required string Documento { get; set; }

        [Required(ErrorMessage = "A senha é obrigatório.")]
        [DataType(DataType.Password)]
        public required string Senha { get; set; }

        public DateTime DataCadastro { get; set; }

        // Último acesso do usuário
        public DateTime? UltimoAcesso { get; set; }

        [Required(ErrorMessage = "O perfil da conta é obrigatório.")]
        public Perfil Perfil { get; set; }

        [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
        public TipoUsuario TipoUsuario { get; set; }

        // Foto de perfil
        [StringLength(500, ErrorMessage = "O caminho da foto deve ter no máximo 500 caracteres.")]
        public string FotoPerfil { get; set; } = "https://ui-avatars.com/api/?name=User&background=3b82f6&color=fff&size=200&bold=true";

        // --- Campo para salvar o modo atual da conta (Pf, Pj, ambos) ---
        [Display(Name = "Preferência de Visualização")]
        public TipoContexto UltimoContexto { get; set; } = TipoContexto.Pessoal; // Padrão

        // Relacionamentos existentes
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public virtual ICollection<Widget> Widgets { get; set; } = new List<Widget>();
        public virtual ICollection<Receita> Receitas { get; set; } = new List<Receita>();
        public virtual ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();

        // ✅ NOVO: Relacionamento com ConversorEnergia
        public virtual ICollection<ConversorEnergia> ConversoresEnergia { get; set; } = new List<ConversorEnergia>();
    }

    public enum Perfil
    {
        Fisica,
        Juridica
    }

    public enum TipoUsuario
    {
        Administrador,
        Moderador,
        Usuario
    }
}