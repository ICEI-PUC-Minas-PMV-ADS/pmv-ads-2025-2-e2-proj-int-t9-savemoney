using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoContabilAndCategoryLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Receita",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Despesa",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoContabil",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1,
                column: "TipoContabil",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2,
                column: "TipoContabil",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3,
                column: "TipoContabil",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4,
                column: "TipoContabil",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5,
                column: "TipoContabil",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 12, 4, 4, 25, 24, 156, DateTimeKind.Local).AddTicks(3864));

            migrationBuilder.CreateIndex(
                name: "IX_Receita_CategoryId",
                table: "Receita",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesa_CategoryId",
                table: "Despesa",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Despesa_Category_CategoryId",
                table: "Despesa",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receita_Category_CategoryId",
                table: "Receita",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesa_Category_CategoryId",
                table: "Despesa");

            migrationBuilder.DropForeignKey(
                name: "FK_Receita_Category_CategoryId",
                table: "Receita");

            migrationBuilder.DropIndex(
                name: "IX_Receita_CategoryId",
                table: "Receita");

            migrationBuilder.DropIndex(
                name: "IX_Despesa_CategoryId",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "TipoContabil",
                table: "Category");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 12, 4, 3, 40, 45, 608, DateTimeKind.Local).AddTicks(4167));
        }
    }
}
