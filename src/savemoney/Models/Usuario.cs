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

        [Required(ErrorMessage = "O documento é obrigatório.")]
        public required string Documento { get; set; }

        [Required(ErrorMessage = "A senha é obrigatório.")]
        [DataType(DataType.Password)]
        public required string Senha { get; set; }

        public DateTime DataCadastro { get; set; }

        [Required(ErrorMessage = "O perfil da conta é obrigatório.")]
        public Perfil Perfil { get; set; }

        [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
        public TipoUsuario TipoUsuario { get; set; }
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
