using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AddMetasFinanceirasEAportes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ConsumoMensal",
                table: "ConversoresEnergia",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateTable(
                name: "MetasFinanceiras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValorObjetivo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorAtual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataLimite = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetasFinanceiras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetasFinanceiras_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataAporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetaFinanceiraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aportes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aportes_MetasFinanceiras_MetaFinanceiraId",
                        column: x => x.MetaFinanceiraId,
                        principalTable: "MetasFinanceiras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aportes_MetaFinanceiraId",
                table: "Aportes",
                column: "MetaFinanceiraId");

            migrationBuilder.CreateIndex(
                name: "IX_MetasFinanceiras_UsuarioId",
                table: "MetasFinanceiras",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aportes");

            migrationBuilder.DropTable(
                name: "MetasFinanceiras");

            migrationBuilder.AlterColumn<double>(
                name: "ConsumoMensal",
                table: "ConversoresEnergia",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
