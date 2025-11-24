using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUserTheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTheme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    NomeTema = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BgPrimary = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BgSecondary = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BgCard = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    TextPrimary = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    TextSecondary = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    AccentPrimary = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    AccentPrimaryHover = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BtnPrimaryText = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTheme", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTheme_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 21, 1, 27, 51, 286, DateTimeKind.Local).AddTicks(4882));

            migrationBuilder.CreateIndex(
                name: "IX_UserTheme_UsuarioId",
                table: "UserTheme",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTheme");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 21, 0, 54, 22, 851, DateTimeKind.Local).AddTicks(486));
        }
    }
}
