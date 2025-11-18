using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System;

namespace savemoney.Data
{
    /// <summary>
    /// Classe auxiliar para popular o banco de dados com dados de teste
    /// Insere 6 meses de histórico financeiro (Receitas e Despesas)
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Método de extensão para popular dados financeiros no ModelBuilder
        /// </summary>
        public static void SeedUserFinancialData(this ModelBuilder modelBuilder)
        {
            // RECEITAS - 6 meses (Jan a Jun 2024)
            modelBuilder.Entity<Receita>().HasData(
                // Janeiro 2024 - R$ 3.000
                new Receita
                {
                    Id = 101,
                    Titulo = "Salário Janeiro",
                    Valor = 3000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 1, 15),
                    DataFim = new DateTime(2024, 1, 15),
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Receita.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // Fevereiro 2024 - R$ 3.200
                new Receita
                {
                    Id = 102,
                    Titulo = "Salário Fevereiro",
                    Valor = 3200.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 2, 15),
                    DataFim = new DateTime(2024, 2, 15),
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Receita.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // Março 2024 - R$ 3.500
                new Receita
                {
                    Id = 103,
                    Titulo = "Salário Março",
                    Valor = 3500.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 3, 15),
                    DataFim = new DateTime(2024, 3, 15),
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Receita.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // Abril 2024 - R$ 3.800
                new Receita
                {
                    Id = 104,
                    Titulo = "Salário Abril",
                    Valor = 3800.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 4, 15),
                    DataFim = new DateTime(2024, 4, 15),
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Receita.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // Maio 2024 - R$ 4.000
                new Receita
                {
                    Id = 105,
                    Titulo = "Salário Maio",
                    Valor = 4000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 5, 15),
                    DataFim = new DateTime(2024, 5, 15),
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Receita.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // Junho 2024 - R$ 4.200
                new Receita
                {
                    Id = 106,
                    Titulo = "Salário Junho",
                    Valor = 4200.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 6, 15),
                    DataFim = new DateTime(2024, 6, 15),
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Receita.RecurrenceType.Monthly,
                    RecurrenceCount = null
                }
            );

            // ========================================
            // DESPESAS - 18 itens (6 meses x 3 categorias)
            // ========================================
            modelBuilder.Entity<Despesa>().HasData(
                // ========== JANEIRO 2024 ==========
                // Moradia: Aluguel
                new Despesa
                {
                    Id = 201,
                    Titulo = "Aluguel Janeiro",
                    Valor = 1000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 1, 5),
                    DataFim = new DateTime(2024, 1, 5),
                    BudgetCategoryId = null, // Sem vínculo com orçamento
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                // Alimentação: Mercado
                new Despesa
                {
                    Id = 202,
                    Titulo = "Mercado Janeiro",
                    Valor = 800.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 1, 10),
                    DataFim = new DateTime(2024, 1, 10),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                // Transporte
                new Despesa
                {
                    Id = 203,
                    Titulo = "Transporte Janeiro",
                    Valor = 300.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 1, 20),
                    DataFim = new DateTime(2024, 1, 20),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // ========== FEVEREIRO 2024 ==========
                new Despesa
                {
                    Id = 204,
                    Titulo = "Aluguel Fevereiro",
                    Valor = 1000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 2, 5),
                    DataFim = new DateTime(2024, 2, 5),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 205,
                    Titulo = "Mercado Fevereiro",
                    Valor = 850.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 2, 10),
                    DataFim = new DateTime(2024, 2, 10),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 206,
                    Titulo = "Transporte Fevereiro",
                    Valor = 350.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 2, 20),
                    DataFim = new DateTime(2024, 2, 20),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // ========== MARÇO 2024 ==========
                new Despesa
                {
                    Id = 207,
                    Titulo = "Aluguel Março",
                    Valor = 1000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 3, 5),
                    DataFim = new DateTime(2024, 3, 5),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 208,
                    Titulo = "Mercado Março",
                    Valor = 900.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 3, 10),
                    DataFim = new DateTime(2024, 3, 10),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 209,
                    Titulo = "Transporte Março",
                    Valor = 400.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 3, 20),
                    DataFim = new DateTime(2024, 3, 20),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // ========== ABRIL 2024 ==========
                new Despesa
                {
                    Id = 210,
                    Titulo = "Aluguel Abril",
                    Valor = 1000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 4, 5),
                    DataFim = new DateTime(2024, 4, 5),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 211,
                    Titulo = "Mercado Abril",
                    Valor = 950.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 4, 10),
                    DataFim = new DateTime(2024, 4, 10),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 212,
                    Titulo = "Transporte Abril",
                    Valor = 450.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 4, 20),
                    DataFim = new DateTime(2024, 4, 20),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // ========== MAIO 2024 ==========
                new Despesa
                {
                    Id = 213,
                    Titulo = "Aluguel Maio",
                    Valor = 1000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 5, 5),
                    DataFim = new DateTime(2024, 5, 5),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 214,
                    Titulo = "Mercado Maio",
                    Valor = 1000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 5, 10),
                    DataFim = new DateTime(2024, 5, 10),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 215,
                    Titulo = "Transporte Maio",
                    Valor = 500.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 5, 20),
                    DataFim = new DateTime(2024, 5, 20),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },

                // ========== JUNHO 2024 ==========
                new Despesa
                {
                    Id = 216,
                    Titulo = "Aluguel Junho",
                    Valor = 1000.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 6, 5),
                    DataFim = new DateTime(2024, 6, 5),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 217,
                    Titulo = "Mercado Junho",
                    Valor = 1050.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 6, 10),
                    DataFim = new DateTime(2024, 6, 10),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                },
                new Despesa
                {
                    Id = 218,
                    Titulo = "Transporte Junho",
                    Valor = 550.00m,
                    CurrencyType = "BRL",
                    DataInicio = new DateTime(2024, 6, 20),
                    DataFim = new DateTime(2024, 6, 20),
                    BudgetCategoryId = null,
                    Recebido = true,
                    IsRecurring = false,
                    Recurrence = Despesa.RecurrenceType.Monthly,
                    RecurrenceCount = null
                }
            );
        }
    }
}

// RESUMO DOS DADOS INSERIDOS

// MÊS 1 (Jan): Receitas R$ 3.000 | Despesas R$ 2.100 | Saldo R$ 900
// MÊS 2 (Fev): Receitas R$ 3.200 | Despesas R$ 2.200 | Saldo R$ 1.000
// MÊS 3 (Mar): Receitas R$ 3.500 | Despesas R$ 2.300 | Saldo R$ 1.200
// MÊS 4 (Abr): Receitas R$ 3.800 | Despesas R$ 2.400 | Saldo R$ 1.400
// MÊS 5 (Mai): Receitas R$ 4.000 | Despesas R$ 2.500 | Saldo R$ 1.500
// MÊS 6 (Jun): Receitas R$ 4.200 | Despesas R$ 2.600 | Saldo R$ 1.600
//
// TENDÊNCIA: CRESCENTE (+77,8% de crescimento do saldo)
