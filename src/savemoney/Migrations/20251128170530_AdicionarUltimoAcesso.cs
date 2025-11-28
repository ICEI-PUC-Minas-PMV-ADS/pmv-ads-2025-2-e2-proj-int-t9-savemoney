using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUltimoAcesso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoAcesso",
                table: "Usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataCadastro", "UltimoAcesso" },
                values: new object[] { new DateTime(2025, 11, 28, 14, 5, 30, 251, DateTimeKind.Local).AddTicks(4841), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UltimoAcesso",
                table: "Usuario");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 27, 4, 13, 8, 251, DateTimeKind.Local).AddTicks(3295));
        }
    }
}
