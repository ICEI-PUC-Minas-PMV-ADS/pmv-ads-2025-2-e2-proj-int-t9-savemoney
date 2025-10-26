using System;
using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public class  Receita
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome da receita é obrigatório.")]
        public string  Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor da receita é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Informe um valor maior que Zero (0)")]
        public double Valor { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Data { get; set; } = DateTime.Now;

        [Display(Name = "Categoria")]
        public string? Categoria { get; set; }

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }
    }
}