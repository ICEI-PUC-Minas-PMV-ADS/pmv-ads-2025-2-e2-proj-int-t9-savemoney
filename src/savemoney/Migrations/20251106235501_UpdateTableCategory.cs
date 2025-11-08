using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIdToCategory_Manual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // VAZIO — já foi feito via SQL
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Opcional: reverter manualmente se quiser
            // migrationBuilder.Sql("ALTER TABLE Category DROP CONSTRAINT FK_Category_Usuario_UsuarioId;");
            // migrationBuilder.DropIndex("IX_Category_UsuarioId", "Category");
            // migrationBuilder.DropColumn("UsuarioId", "Category");
        }
    }
}