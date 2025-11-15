using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class ReceitasEDespesas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Receitas",
                table: "Receitas");

            migrationBuilder.DropIndex(
                name: "IX_Receitas_BudgetCategoryId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndDate",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "BudgetCategoryId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndDate",
                table: "Receitas");

            migrationBuilder.RenameTable(
                name: "Receitas",
                newName: "Receita");

            migrationBuilder.RenameColumn(
                name: "RecurrenceOccurrences",
                table: "Despesa",
                newName: "RecurrenceCount");

            migrationBuilder.RenameColumn(
                name: "Interval",
                table: "Despesa",
                newName: "Recurrence");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Despesa",
                newName: "DataInicio");

            migrationBuilder.RenameColumn(
                name: "RecurrenceOccurrences",
                table: "Receita",
                newName: "RecurrenceCount");

            migrationBuilder.RenameColumn(
                name: "Interval",
                table: "Receita",
                newName: "Recurrence");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Receita",
                newName: "DataInicio");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Despesa",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Despesa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyType",
                table: "Despesa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Despesa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Recebido",
                table: "Despesa",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Receita",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Receita",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyType",
                table: "Receita",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Receita",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Recebido",
                table: "Receita",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receita",
                table: "Receita",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Receita",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "CurrencyType",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "Recebido",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "CurrencyType",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "Recebido",
                table: "Receita");

            migrationBuilder.RenameTable(
                name: "Receita",
                newName: "Receitas");

            migrationBuilder.RenameColumn(
                name: "RecurrenceCount",
                table: "Despesa",
                newName: "RecurrenceOccurrences");

            migrationBuilder.RenameColumn(
                name: "Recurrence",
                table: "Despesa",
                newName: "Interval");

            migrationBuilder.RenameColumn(
                name: "DataInicio",
                table: "Despesa",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "RecurrenceCount",
                table: "Receitas",
                newName: "RecurrenceOccurrences");

            migrationBuilder.RenameColumn(
                name: "Recurrence",
                table: "Receitas",
                newName: "Interval");

            migrationBuilder.RenameColumn(
                name: "DataInicio",
                table: "Receitas",
                newName: "Data");

            migrationBuilder.AlterColumn<double>(
                name: "Valor",
                table: "Despesa",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Despesa",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Despesa",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Despesa",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurrenceEndDate",
                table: "Despesa",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Valor",
                table: "Receitas",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Receitas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "BudgetCategoryId",
                table: "Receitas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Receitas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Receitas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurrenceEndDate",
                table: "Receitas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receitas",
                table: "Receitas",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_BudgetCategoryId",
                table: "Receitas",
                column: "BudgetCategoryId");
        }
    }
}
