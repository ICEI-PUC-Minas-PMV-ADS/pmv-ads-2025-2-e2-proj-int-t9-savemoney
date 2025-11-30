using savemoney.Models;

namespace savemoney.Services
{
    public static class ServicoCalculoRecorrencia
    {
        // Método Principal: Recebe os dados brutos e devolve a lista pronta para o relatório
        public static List<TransacaoProcessada> ProcessarTransacoes(
            List<Receita> receitas,
            List<Despesa> despesas,
            DateTime dataInicioRelatorio,
            DateTime dataFimRelatorio)
        {
            var listaProcessada = new List<TransacaoProcessada>();

            // 1. Processar Receitas
            foreach (var receita in receitas)
            {
                var datasOcorrencia = CalcularDatasOcorrencia(
                    receita.DataInicio,
                    receita.DataFim,
                    receita.IsRecurring,
                    receita.Recurrence, // Enum original da Receita
                    receita.RecurrenceCount,
                    dataInicioRelatorio,
                    dataFimRelatorio
                );

                foreach (var data in datasOcorrencia)
                {
                    listaProcessada.Add(new TransacaoProcessada
                    {
                        Data = data,
                        Valor = receita.Valor,
                        Descricao = receita.Titulo,
                        Categoria = "Receita",
                        EhReceita = true
                    });
                }
            }

            // 2. Processar Despesas
            foreach (var despesa in despesas)
            {
                var datasOcorrencia = CalcularDatasOcorrencia(
                    despesa.DataInicio,
                    despesa.DataFim,
                    despesa.IsRecurring,
                    despesa.Recurrence, // Enum original da Despesa
                    despesa.RecurrenceCount,
                    dataInicioRelatorio,
                    dataFimRelatorio
                );

                foreach (var data in datasOcorrencia)
                {
                    listaProcessada.Add(new TransacaoProcessada
                    {
                        Data = data,
                        Valor = despesa.Valor,
                        Descricao = despesa.Titulo,
                        // Se a categoria foi excluída, exibe "Sem Categoria"
                        Categoria = despesa.BudgetCategory?.Category?.Name ?? "Sem Categoria",
                        EhReceita = false
                    });
                }
            }

            // Retorna ordenado por data (Cronológico)
            return listaProcessada.OrderBy(t => t.Data).ToList();
        }

        // Método Auxiliar: Calcula as datas futuras baseadas na recorrência
        // Usamos 'dynamic' no tipoRecorrencia para aceitar tanto o Enum de Receita quanto de Despesa
        private static List<DateTime> CalcularDatasOcorrencia(
            DateTime inicioItem,
            DateTime fimItem,
            bool ehRecorrente,
            dynamic tipoRecorrencia,
            int? qtdRepeticoes,
            DateTime filtroInicio,
            DateTime filtroFim)
        {
            var datas = new List<DateTime>();
            var dataAtual = inicioItem.Date;

            // Se NÃO é recorrente, só retorna a data se estiver dentro do período do relatório
            if (!ehRecorrente)
            {
                if (dataAtual >= filtroInicio && dataAtual <= filtroFim)
                {
                    datas.Add(dataAtual);
                }
                return datas;
            }

            // Configuração de limites para evitar loops infinitos
            int maxRepeticoes = (qtdRepeticoes.HasValue && qtdRepeticoes.Value > 0) ? qtdRepeticoes.Value : 999;

            // Define a data limite final (o que ocorrer primeiro: data fim do item ou um limite de segurança de 5 anos)
            DateTime limiteSeguranca = DateTime.Today.AddYears(5);
            DateTime dataLimiteReal = (fimItem != default && fimItem != DateTime.MinValue) ? fimItem.Date : limiteSeguranca;

            if (dataLimiteReal > limiteSeguranca) dataLimiteReal = limiteSeguranca;

            // Loop de cálculo
            for (int i = 0; i < maxRepeticoes && dataAtual <= dataLimiteReal; i++)
            {
                // Só adiciona na lista final se a data cair DENTRO do período do relatório visualizado
                if (dataAtual >= filtroInicio && dataAtual <= filtroFim)
                {
                    datas.Add(dataAtual);
                }

                // Se já passou do final do relatório, interrompe para economizar processamento
                if (dataAtual > filtroFim) break;

                // Avança para a próxima data baseada no Enum
                // Convertendo para string para comparar, pois os Enums podem ser tipos diferentes (Receita.Recurrence vs Despesa.Recurrence)
                string tipo = tipoRecorrencia.ToString();

                if (tipo == "Diario" || tipo == "Daily") dataAtual = dataAtual.AddDays(1);
                else if (tipo == "Semanal" || tipo == "Weekly") dataAtual = dataAtual.AddDays(7);
                else if (tipo == "Mensal" || tipo == "Monthly") dataAtual = dataAtual.AddMonths(1);
                else if (tipo == "Anual" || tipo == "Yearly") dataAtual = dataAtual.AddYears(1);
                else dataAtual = dataAtual.AddMonths(1); // Padrão
            }

            return datas;
        }
    }
}