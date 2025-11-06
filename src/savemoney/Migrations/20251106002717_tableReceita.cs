using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class tableReceita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Receitas");

            migrationBuilder.AddColumn<int>(
                name: "BudgetCategoryId",
                table: "Receitas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Receitas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "Receitas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Receitas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurrenceEndDate",
                table: "Receitas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceOccurrences",
                table: "Receitas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_BudgetCategoryId",
                table: "Receitas",
                column: "BudgetCategoryId");

           // migrationBuilder.AddForeignKey(
             //   name: "FK_Receitas_BudgetCategory_BudgetCategoryId",
          //      table: "Receitas",
           //     column: "BudgetCategoryId",
           //     principalTable: "BudgetCategory",
           //     principalColumn: "Id",
           //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
     //       migrationBuilder.DropForeignKey(
       //         name: "FK_Receitas_BudgetCategory_BudgetCategoryId",
         //       table: "Receitas");

            migrationBuilder.DropIndex(
                name: "IX_Receitas_BudgetCategoryId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "BudgetCategoryId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndDate",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "RecurrenceOccurrences",
                table: "Receitas");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Receitas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
