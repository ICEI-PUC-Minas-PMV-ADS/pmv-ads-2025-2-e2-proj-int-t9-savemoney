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
        public required string Email { get; set; }
        [Required(ErrorMessage = "A senha é obrigatório.")]
        [DataType(DataType.Password)]
        public required string Senha { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        [Required(ErrorMessage = "O perfil é obrigatório.")]
        public Perfil Perfil { get; set; }
    }

    public enum Perfil {
        Administrador,
        UsuarioComum
    }
}
