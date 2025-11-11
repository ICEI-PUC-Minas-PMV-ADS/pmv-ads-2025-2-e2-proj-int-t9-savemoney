using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class NovaMigracaoCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_BudgetCategory_BudgetCategoryId",
                table: "Receitas");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetCategoryId",
                table: "Receitas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1,
                column: "UsuarioId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2,
                column: "UsuarioId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3,
                column: "UsuarioId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4,
                column: "UsuarioId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5,
                column: "UsuarioId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Category_UsuarioId",
                table: "Category",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Usuario_UsuarioId",
                table: "Category",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_BudgetCategory_BudgetCategoryId",
                table: "Receitas",
                column: "BudgetCategoryId",
                principalTable: "BudgetCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Usuario_UsuarioId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_BudgetCategory_BudgetCategoryId",
                table: "Receitas");

            migrationBuilder.DropIndex(
                name: "IX_Category_UsuarioId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Category");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetCategoryId",
                table: "Receitas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_BudgetCategory_BudgetCategoryId",
                table: "Receitas",
                column: "BudgetCategoryId",
                principalTable: "BudgetCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
