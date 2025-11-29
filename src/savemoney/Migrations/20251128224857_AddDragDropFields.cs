using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AddDragDropFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "Widget",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisivel",
                table: "Widget",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TipoWidget",
                table: "Widget",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaMovimentacao",
                table: "Widget",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ZIndex",
                table: "Widget",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 28, 19, 48, 56, 777, DateTimeKind.Local).AddTicks(1950));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "Widget");

            migrationBuilder.DropColumn(
                name: "IsVisivel",
                table: "Widget");

            migrationBuilder.DropColumn(
                name: "TipoWidget",
                table: "Widget");

            migrationBuilder.DropColumn(
                name: "UltimaMovimentacao",
                table: "Widget");

            migrationBuilder.DropColumn(
                name: "ZIndex",
                table: "Widget");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 11, 28, 14, 45, 48, 582, DateTimeKind.Local).AddTicks(7656));
        }
    }
}
