using Microsoft.EntityFrameworkCore;
using savemoney.Data;

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
        public DbSet<Receita> Receitas { get; set; } = default!;

        // Correto: apenas UM DbSet para Despesa
        public DbSet<Despesa> Despesas { get; set; } = default!;

        // Novos DbSets para Metas Financeiras
        public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }
        public DbSet<Aporte> Aportes { get; set; }

        // Gestão de Orçamento
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MetaFinanceira → Aportes (Cascade)
            modelBuilder.Entity<MetaFinanceira>()
                .HasMany(m => m.Aportes)
                .WithOne(a => a.MetaFinanceira)
                .HasForeignKey(a => a.MetaFinanceiraId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category → Usuario
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.Usuario)
                      .WithMany(u => u.Categories)
                      .HasForeignKey(c => c.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Budget → Usuario
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasOne(b => b.Usuario)
                      .WithMany(u => u.Budgets)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Budget → BudgetCategory (Cascade)
            modelBuilder.Entity<Budget>()
                .HasMany(b => b.Categories)
                .WithOne(bc => bc.Budget)
                .HasForeignKey(bc => bc.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category → BudgetCategory (Restrict)
           modelBuilder.Entity<Category>()
                .HasMany(c => c.BudgetCategories)
                .WithOne(bc => bc.Category)
                .HasForeignKey(bc => bc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dentro do OnModelCreating
            /*modelBuilder.Entity<BudgetCategory>()
                .HasMany(bc => bc.Despesas)
                .WithOne(d => d.BudgetCategory) 
                // ← Certifique-se que Despesa tem essa propriedade!
                .HasForeignKey(d => d.BudgetCategoryId) 
                .OnDelete(DeleteBehavior.Cascade);*/

            // Categorias padrão
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                    {
                        Id = 1, 
                        Nome = "Admin Savemoney",
                        Email = "admin@savemoney.com",
                        Senha = "123456", // Lembrete: Placeholder! Em produção, use hashing!
                        Documento = "000.000.000-00",
                        Perfil = 0,
                        TipoUsuario = 0,
                    DataCadastro = DateTime.Now
                    }
                );
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Alimentação", IsPredefined = true },
                new Category { Id = 2, Name = "Lazer", IsPredefined = true },
                new Category { Id = 3, Name = "Transporte", IsPredefined = true },
                new Category { Id = 4, Name = "Moradia", IsPredefined = true },
                new Category { Id = 5, Name = "Despesas Operacionais", IsPredefined = true }
            );
            //modelBuilder.SeedUserFinancialData();
        }
    }
}
