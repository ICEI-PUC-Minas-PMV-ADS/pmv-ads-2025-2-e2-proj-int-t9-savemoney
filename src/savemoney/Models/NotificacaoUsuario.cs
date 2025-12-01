using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    public enum TipoNotificacao
    {
        Info,           // Informações gerais
        Sucesso,        // Ações concluídas (ex: Receita salva)
        AlertaOrcamento, // R11: Gastos próximos ou acima do limite
        ContaPendente,   // R11: Contas a pagar/receber próximas
        Erro,           // Falhas
        Sistema         // Updates via JSON
    }

    public class NotificacaoUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(500)]
        public string Mensagem { get; set; }

        [Required]
        public TipoNotificacao Tipo { get; set; } = TipoNotificacao.Info;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public bool Lida { get; set; } = false;

        // Para evitar duplicidade ao importar do JSON de sistema
        [StringLength(50)]
        public string? CodigoReferenciaSistema { get; set; }

        // R11: Link opcional para ação corretiva (ex: ir para o orçamento estourado)
        [StringLength(200)]
        public string? LinkAcao { get; set; }
    }
}