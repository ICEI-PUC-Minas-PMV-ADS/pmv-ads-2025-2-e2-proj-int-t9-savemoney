using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIdToReceitasEDespesas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Receita",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Despesa",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 22, 0, 58, 2, 799, DateTimeKind.Local).AddTicks(8475));

            migrationBuilder.CreateIndex(
                name: "IX_Receita_UsuarioId",
                table: "Receita",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesa_UsuarioId",
                table: "Despesa",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Despesa_Usuario_UsuarioId",
                table: "Despesa",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receita_Usuario_UsuarioId",
                table: "Receita",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesa_Usuario_UsuarioId",
                table: "Despesa");

            migrationBuilder.DropForeignKey(
                name: "FK_Receita_Usuario_UsuarioId",
                table: "Receita");

            migrationBuilder.DropIndex(
                name: "IX_Receita_UsuarioId",
                table: "Receita");

            migrationBuilder.DropIndex(
                name: "IX_Despesa_UsuarioId",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Despesa");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 21, 1, 27, 51, 286, DateTimeKind.Local).AddTicks(4882));
        }
    }
}
