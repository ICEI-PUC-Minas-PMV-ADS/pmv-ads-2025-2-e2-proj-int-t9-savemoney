using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace savemoney.Migrations
{
    public partial class AddCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cria a tabela Category
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPredefined = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            // Insere as 5 categorias predefinidas
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name", "IsPredefined" },
                values: new object[,]
                {
                    { 1, "Alimentação", true },
                    { 2, "Lazer", true },
                    { 3, "Transporte", true },
                    { 4, "Moradia", true },
                    { 5, "Despesas Operacionais", true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove a tabela se a migration for revertida
            migrationBuilder.DropTable(name: "Category");
        }
    }
}