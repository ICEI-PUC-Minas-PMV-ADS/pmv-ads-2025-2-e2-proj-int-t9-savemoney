using System;
using System.Collections.Generic;

namespace savemoney.Models.ViewModels
{
    public class RelatorioFinanceiroViewModel
    {
        // Filtros aplicados
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        // KPIs (Indicadores)
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo { get; set; }
        public decimal TaxaPoupanca { get; set; }

        // Lista detalhada processada (para tabelas)
        public List<TransacaoProcessada> TransacoesDetalhadas { get; set; } = new();

        // Lista de mensagens automáticas
        public List<string> Diagnosticos { get; set; } = new();

        // Dados formatados para JSON (para uso nos gráficos JS)
        public string JsonLabelsTimeline { get; set; }      // Datas (Eixo X)
        public string JsonDadosReceitas { get; set; }       // Valores Receitas (Eixo Y1)
        public string JsonDadosDespesas { get; set; }       // Valores Despesas (Eixo Y2)
        public string JsonLabelsCategorias { get; set; }    // Nomes Categorias (Rosca)
        public string JsonValoresCategorias { get; set; }   // Valores Categorias (Rosca)
    }
}