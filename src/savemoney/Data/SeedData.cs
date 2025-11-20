using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System;

namespace savemoney.Data
{
    public static class SeedData
    {
        public static void SeedUserFinancialData(this ModelBuilder modelBuilder)
        {
            // RECEITAS - 6 meses (Jan a Jun 2024)
            modelBuilder.Entity<Receita>().HasData(
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

            // DESPESAS
            modelBuilder.Entity<Despesa>().HasData(
                // Janeiro
                new Despesa { Id = 201, Titulo = "Aluguel Janeiro", Valor = 1000.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 1, 5), DataFim = new DateTime(2024, 1, 5), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 202, Titulo = "Mercado Janeiro", Valor = 800.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 1, 10), DataFim = new DateTime(2024, 1, 10), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 203, Titulo = "Transporte Janeiro", Valor = 300.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 1, 20), DataFim = new DateTime(2024, 1, 20), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                // Fevereiro
                new Despesa { Id = 204, Titulo = "Aluguel Fevereiro", Valor = 1000.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 2, 5), DataFim = new DateTime(2024, 2, 5), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 205, Titulo = "Mercado Fevereiro", Valor = 850.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 2, 10), DataFim = new DateTime(2024, 2, 10), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 206, Titulo = "Transporte Fevereiro", Valor = 350.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 2, 20), DataFim = new DateTime(2024, 2, 20), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                // Março
                new Despesa { Id = 207, Titulo = "Aluguel Março", Valor = 1000.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 3, 5), DataFim = new DateTime(2024, 3, 5), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 208, Titulo = "Mercado Março", Valor = 900.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 3, 10), DataFim = new DateTime(2024, 3, 10), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 209, Titulo = "Transporte Março", Valor = 400.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 3, 20), DataFim = new DateTime(2024, 3, 20), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                // Abril
                new Despesa { Id = 210, Titulo = "Aluguel Abril", Valor = 1000.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 4, 5), DataFim = new DateTime(2024, 4, 5), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 211, Titulo = "Mercado Abril", Valor = 950.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 4, 10), DataFim = new DateTime(2024, 4, 10), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 212, Titulo = "Transporte Abril", Valor = 450.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 4, 20), DataFim = new DateTime(2024, 4, 20), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                // Maio
                new Despesa { Id = 213, Titulo = "Aluguel Maio", Valor = 1000.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 5, 5), DataFim = new DateTime(2024, 5, 5), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 214, Titulo = "Mercado Maio", Valor = 1000.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 5, 10), DataFim = new DateTime(2024, 5, 10), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 215, Titulo = "Transporte Maio", Valor = 500.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 5, 20), DataFim = new DateTime(2024, 5, 20), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                // Junho
                new Despesa { Id = 216, Titulo = "Aluguel Junho", Valor = 1000.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 6, 5), DataFim = new DateTime(2024, 6, 5), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 217, Titulo = "Mercado Junho", Valor = 1050.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 6, 10), DataFim = new DateTime(2024, 6, 10), BudgetCategoryId = null, Pago  = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null },
                new Despesa { Id = 218, Titulo = "Transporte Junho", Valor = 550.00m, CurrencyType = "BRL", DataInicio = new DateTime(2024, 6, 20), DataFim = new DateTime(2024, 6, 20), BudgetCategoryId = null, Pago = true, IsRecurring = false, Recurrence = Despesa.RecurrenceType.Monthly, RecurrenceCount = null }
            );
        }
    }
}