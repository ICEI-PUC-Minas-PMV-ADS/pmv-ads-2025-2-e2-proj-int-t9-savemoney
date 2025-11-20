using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AddWidgetsAndFotoPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoPerfil",
                table: "Usuario",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Receita",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Despesa",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "Widget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Link = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Colunas = table.Column<int>(type: "int", nullable: false),
                    Largura = table.Column<int>(type: "int", nullable: false),
                    PosicaoX = table.Column<int>(type: "int", nullable: false),
                    PosicaoY = table.Column<int>(type: "int", nullable: false),
                    CorFundo = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widget_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataCadastro", "FotoPerfil" },
                values: new object[] { new DateTime(2025, 11, 20, 13, 34, 59, 451, DateTimeKind.Local).AddTicks(5041), "https://ui-avatars.com/api/?name=Admin+Savemoney&background=3b82f6&color=fff&size=200&bold=true" });

            migrationBuilder.CreateIndex(
                name: "IX_Widget_UsuarioId",
                table: "Widget",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Widget");

            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Usuario");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Receita",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Despesa",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 20, 7, 12, 17, 458, DateTimeKind.Local).AddTicks(8700));
        }
    }
}
