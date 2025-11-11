using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets existentes
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ConversorEnergia> ConversoresEnergia { get; set; }

        // Dbset para Receitas e Despesas a fim de resolver os bugs no compilador após o merge
        public DbSet<Receita> Receitas { get; set; }

        // Novos DbSets para o R9 - Metas Financeiras
        public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }
        public DbSet<Aporte> Aportes { get; set; }

        // Novos DbSets para o R12 - Gestão de Orçamento
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }

        // Método para configurar os relacionamentos e comportamentos do banco de dados
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração crucial (Exclusão em Cascata): 
            // Quando uma MetaFinanceira for deletada, todos os Aportes relacionados a ela 
            // devem ser automaticamente deletados pelo banco de dados.
            modelBuilder.Entity<MetaFinanceira>()
                .HasMany(m => m.Aportes)          // Uma Meta tem muitos Aportes
                .WithOne(a => a.MetaFinanceira)   // Um Aporte pertence a uma Meta
                .HasForeignKey(a => a.MetaFinanceiraId) // A chave estrangeira é MetaFinanceiraId
                .OnDelete(DeleteBehavior.Cascade); // Habilita a exclusão em cascata

            // === CONFIGURAÇÃO DA FK: Category → Usuario ===
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.Usuario)
                      .WithMany(u => u.Categories)  // Adicione esta propriedade no Usuario se não existir
                      .HasForeignKey(c => c.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuração para Budget e BudgetCategory: Exclusão em cascata
            modelBuilder.Entity<Budget>()
                .HasMany(b => b.Categories)
                .WithOne(bc => bc.Budget)
                .HasForeignKey(bc => bc.BudgetId)
                .OnDelete(DeleteBehavior.Cascade); // Quando um Budget é deletado, suas BudgetCategories são deletadas

            // Configuração para Category e BudgetCategory: Restringir exclusão
            modelBuilder.Entity<Category>()
                .HasMany(c => c.BudgetCategories)
                .WithOne(bc => bc.Category)
                .HasForeignKey(bc => bc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Impede a exclusão de Category se estiver em uso

            // Seed Data: Categorias predefinidas
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Alimentação", IsPredefined = true },
                new Category { Id = 2, Name = "Lazer", IsPredefined = true },
                new Category { Id = 3, Name = "Transporte", IsPredefined = true },
                new Category { Id = 4, Name = "Moradia", IsPredefined = true },
                new Category { Id = 5, Name = "Despesas Operacionais", IsPredefined = true }
            );
        }
        public DbSet<savemoney.Models.Despesa> Despesa { get; set; } = default!;

    }
}