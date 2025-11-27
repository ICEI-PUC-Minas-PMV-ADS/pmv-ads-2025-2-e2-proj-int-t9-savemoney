using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Services
{
    // Gera o ViewModel agregando receitas e despesas (expande recorrências)
    public static class ReportService
    {
        public static RelatorioViewModel BuildReport(AppDbContext db, int userId, DateTime start, DateTime end, AggregationPeriod period)
        {
            if (start > end) (start, end) = (end, start);

            var receitas = db.Receitas.AsNoTracking().Where(r => r.UsuarioId == userId).ToList();
            var despesas = db.Despesas.AsNoTracking().Where(d => d.UsuarioId == userId).ToList();

            var receitaOccurrences = receitas
                .SelectMany(r => ExpandOccurrences(r, start, end).Select(dt => (Date: dt, Value: (double)r.Valor)));

            var despesaOccurrences = despesas
                .SelectMany(d => ExpandOccurrences(d, start, end).Select(dt => (Date: dt, Value: (double)d.Valor)));

            IEnumerable<KeyValuePair<DateTime, (double receitas, double despesas)>> grouped;

            switch (period)
            {
                case AggregationPeriod.Weekly:
                    grouped = GroupByPeriod(receitaOccurrences, despesaOccurrences, dt => StartOfWeek(dt));
                    break;
                case AggregationPeriod.Yearly:
                    grouped = GroupByPeriod(receitaOccurrences, despesaOccurrences, dt => new DateTime(dt.Year, 1, 1));
                    break;
                default:
                    grouped = GroupByPeriod(receitaOccurrences, despesaOccurrences, dt => new DateTime(dt.Year, dt.Month, 1));
                    break;
            }

            var vm = new RelatorioViewModel { Request = new RelatorioRequest { StartDate = start, EndDate = end, Period = period } };

            var periodos = grouped
                .Select(kv => new PeriodoViewModel
                {
                    Label = FormatLabel(kv.Key, period),
                    Data = kv.Key,
                    TotalReceitas = kv.Value.receitas,
                    TotalDespesas = kv.Value.despesas
                })
                .OrderBy(p => p.Data)
                .ToList();

            for (int i = 0; i < periodos.Count; i++)
            {
                if (i == 0) { periodos[i].VariacaoPercentual = null; continue; }
                var prev = periodos[i - 1];
                var curr = periodos[i];
                periodos[i].VariacaoPercentual =
                    Math.Abs(prev.Saldo) < 0.0001 ? null : (double?)((curr.Saldo - prev.Saldo) / Math.Abs(prev.Saldo) * 100.0);
            }

            vm.Periodos = periodos;
            return vm;
        }

        private static IEnumerable<KeyValuePair<DateTime, (double receitas, double despesas)>> GroupByPeriod(
            IEnumerable<(DateTime Date, double Value)> receitas,
            IEnumerable<(DateTime Date, double Value)> despesas,
            Func<DateTime, DateTime> keySelector)
        {
            var r = receitas.GroupBy(x => keySelector(x.Date)).ToDictionary(g => g.Key, g => g.Sum(x => x.Value));
            var d = despesas.GroupBy(x => keySelector(x.Date)).ToDictionary(g => g.Key, g => g.Sum(x => x.Value));
            var keys = r.Keys.Union(d.Keys).OrderBy(k => k);

            foreach (var k in keys)
            {
                r.TryGetValue(k, out var rv);
                d.TryGetValue(k, out var dv);
                yield return new KeyValuePair<DateTime, (double, double)>(k, (rv, dv));
            }
        }

        private static string FormatLabel(DateTime key, AggregationPeriod period) =>
            period switch
            {
                AggregationPeriod.Weekly => $"{key:dd/MM/yyyy} - {key.AddDays(6):dd/MM/yyyy}",
                AggregationPeriod.Yearly => key.Year.ToString(),
                _ => key.ToString("MMM/yyyy")
            };

        private static DateTime StartOfWeek(DateTime dt, DayOfWeek start = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - start)) % 7;
            return dt.AddDays(-diff).Date;
        }

        private static IEnumerable<DateTime> ExpandOccurrences(Receita r, DateTime start, DateTime end)
        {
            var list = new List<DateTime>();
            var current = r.DataInicio.Date;
            if (r.IsRecurring)
            {
                int max = r.RecurrenceCount ?? 365;
                DateTime limit = r.DataFim != default ? r.DataFim.Date : DateTime.MaxValue;
                for (int i = 0; i < max && current <= end && current <= limit; i++)
                {
                    if (current >= start) list.Add(current);
                    current = r.Recurrence switch
                    {
                        Receita.RecurrenceType.Daily => current.AddDays(1),
                        Receita.RecurrenceType.Weekly => current.AddDays(7),
                        Receita.RecurrenceType.Monthly => current.AddMonths(1),
                        Receita.RecurrenceType.Yearly => current.AddYears(1),
                        _ => current.AddMonths(1)
                    };
                }
            }
            else
            {
                if (current >= start && current <= end) list.Add(current);
            }
            return list;
        }

        private static IEnumerable<DateTime> ExpandOccurrences(Despesa d, DateTime start, DateTime end)
        {
            var list = new List<DateTime>();
            var current = d.DataInicio.Date;
            if (d.IsRecurring)
            {
                int max = d.RecurrenceCount ?? 365;
                DateTime limit = d.DataFim != default ? d.DataFim.Date : DateTime.MaxValue;
                for (int i = 0; i < max && current <= end && current <= limit; i++)
                {
                    if (current >= start) list.Add(current);
                    current = d.Recurrence switch
                    {
                        Despesa.RecurrenceType.Daily => current.AddDays(1),
                        Despesa.RecurrenceType.Weekly => current.AddDays(7),
                        Despesa.RecurrenceType.Monthly => current.AddMonths(1),
                        Despesa.RecurrenceType.Yearly => current.AddYears(1),
                        _ => current.AddMonths(1)
                    };
                }
            }
            else
            {
                if (current >= start && current <= end) list.Add(current);
            }
            return list;
        }
    }
}