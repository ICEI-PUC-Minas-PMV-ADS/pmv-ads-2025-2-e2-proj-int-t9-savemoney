using System;
using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public enum AggregationPeriod { Weekly, Monthly, Yearly }

    public class RelatorioRequest
    {
        [DataType(DataType.Date)]
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-3);

        [DataType(DataType.Date)]
        [Required]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Required]
        public AggregationPeriod Period { get; set; } = AggregationPeriod.Monthly;
    }
}