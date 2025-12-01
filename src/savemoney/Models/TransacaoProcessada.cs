using System;
using System.Globalization;

namespace savemoney.Models
{
    /// <summary>
    /// Objeto processado para exibição em relatórios e gráficos
    /// Representa uma transação individual (receita ou despesa) já calculada
    /// </summary>
    public class TransacaoProcessada
    {
        #region Propriedades Básicas

        /// <summary>
        /// Data da transação (pode ser data original ou data calculada de recorrência)
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Valor monetário da transação
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Título/Descrição da receita ou despesa
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Nome da categoria (para despesas) ou "Receita" (para receitas)
        /// </summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// True = Receita, False = Despesa
        /// </summary>
        public bool EhReceita { get; set; }

        /// <summary>
        /// Indica se a transação original é recorrente
        /// </summary>
        public bool IsRecorrente { get; set; }

        #endregion

        #region Propriedades Calculadas para UI

        /// <summary>
        /// Dia da semana em português
        /// </summary>
        public string DiaSemana => Data.DayOfWeek switch
        {
            DayOfWeek.Sunday => "Domingo",
            DayOfWeek.Monday => "Segunda",
            DayOfWeek.Tuesday => "Terça",
            DayOfWeek.Wednesday => "Quarta",
            DayOfWeek.Thursday => "Quinta",
            DayOfWeek.Friday => "Sexta",
            DayOfWeek.Saturday => "Sábado",
            _ => "Indefinido"
        };

        /// <summary>
        /// Abreviação do dia da semana
        /// </summary>
        public string DiaSemanaAbrev => Data.DayOfWeek switch
        {
            DayOfWeek.Sunday => "Dom",
            DayOfWeek.Monday => "Seg",
            DayOfWeek.Tuesday => "Ter",
            DayOfWeek.Wednesday => "Qua",
            DayOfWeek.Thursday => "Qui",
            DayOfWeek.Friday => "Sex",
            DayOfWeek.Saturday => "Sáb",
            _ => "?"
        };

        /// <summary>
        /// Classe CSS para status (text-success para receita, text-danger para despesa)
        /// </summary>
        public string ClasseCssStatus => EhReceita ? "text-success" : "text-danger";

        /// <summary>
        /// Classe CSS para badge de tipo
        /// </summary>
        public string ClasseCssBadge => EhReceita ? "badge-success" : "badge-danger";

        /// <summary>
        /// Classe CSS para fundo de linha na tabela
        /// </summary>
        public string ClasseCssLinha => EhReceita ? "row-receita" : "row-despesa";

        /// <summary>
        /// Ícone Material Symbols para o tipo de transação
        /// </summary>
        public string Icone => EhReceita ? "arrow_upward" : "arrow_downward";

        /// <summary>
        /// Ícone alternativo para tipo
        /// </summary>
        public string IconeTipo => EhReceita ? "trending_up" : "trending_down";

        /// <summary>
        /// Ícone para recorrência
        /// </summary>
        public string IconeRecorrencia => IsRecorrente ? "repeat" : "event";

        /// <summary>
        /// Tipo da transação em texto
        /// </summary>
        public string TipoTexto => EhReceita ? "Receita" : "Despesa";

        /// <summary>
        /// Valor formatado em moeda brasileira
        /// </summary>
        public string ValorFormatado => Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

        /// <summary>
        /// Valor formatado com sinal (+ para receita, - para despesa)
        /// </summary>
        public string ValorComSinal => EhReceita
            ? $"+{ValorFormatado}"
            : $"-{ValorFormatado}";

        /// <summary>
        /// Data formatada no padrão brasileiro
        /// </summary>
        public string DataFormatada => Data.ToString("dd/MM/yyyy");

        /// <summary>
        /// Data formatada curta (sem ano)
        /// </summary>
        public string DataCurta => Data.ToString("dd/MM");

        /// <summary>
        /// Data e hora formatadas
        /// </summary>
        public string DataHoraFormatada => Data.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// Data relativa (Hoje, Ontem, etc.)
        /// </summary>
        public string DataRelativa
        {
            get
            {
                var hoje = DateTime.Today;
                var dataTransacao = Data.Date;

                if (dataTransacao == hoje) return "Hoje";
                if (dataTransacao == hoje.AddDays(-1)) return "Ontem";
                if (dataTransacao == hoje.AddDays(1)) return "Amanhã";
                if (dataTransacao > hoje.AddDays(-7) && dataTransacao < hoje) return DiaSemana;

                return DataFormatada;
            }
        }

        /// <summary>
        /// Mês/Ano da transação
        /// </summary>
        public string MesAno => Data.ToString("MMM/yyyy", CultureInfo.GetCultureInfo("pt-BR"));

        /// <summary>
        /// Semana do ano
        /// </summary>
        public int SemanaAno
        {
            get
            {
                var cal = CultureInfo.GetCultureInfo("pt-BR").Calendar;
                return cal.GetWeekOfYear(Data, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            }
        }

        /// <summary>
        /// Indica se a transação é do mês atual
        /// </summary>
        public bool EhMesAtual => Data.Year == DateTime.Today.Year && Data.Month == DateTime.Today.Month;

        /// <summary>
        /// Indica se a transação é de hoje
        /// </summary>
        public bool EhHoje => Data.Date == DateTime.Today;

        /// <summary>
        /// Tooltip com informações completas
        /// </summary>
        public string TooltipCompleto => $"{TipoTexto}: {Descricao}\n{ValorFormatado}\n{DataFormatada} ({DiaSemana})\nCategoria: {Categoria}";

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Retorna uma representação em string para debug
        /// </summary>
        public override string ToString()
        {
            return $"[{DataFormatada}] {TipoTexto}: {Descricao} - {ValorFormatado} ({Categoria})";
        }

        /// <summary>
        /// Verifica se a transação está dentro de um período
        /// </summary>
        public bool EstaDentroPeriodo(DateTime inicio, DateTime fim)
        {
            return Data.Date >= inicio.Date && Data.Date <= fim.Date;
        }

        #endregion
    }
}