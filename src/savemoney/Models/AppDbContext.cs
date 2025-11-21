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
        public DbSet<Despesa> Despesas { get; set; } = default!;
        public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }
        public DbSet<Aporte> Aportes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }

        // NOVO: DbSet para Widgets
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<UserTheme> UserThemes { get; set; }

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

            // NOVO: Widget → Usuario (Cascade - se deletar usuário, deleta widgets)
            modelBuilder.Entity<Widget>()
                .HasOne(w => w.Usuario)
                .WithMany(u => u.Widgets)
                .HasForeignKey(w => w.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice para melhorar performance nas queries de widgets por usuário
            modelBuilder.Entity<Widget>()
                .HasIndex(w => w.UsuarioId);

            // Dados seed existentes
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Admin Savemoney",
                    Email = "admin@savemoney.com",
                    Senha = "123456",
                    Documento = "000.000.000-00",
                    Perfil = 0,
                    TipoUsuario = 0,
                    DataCadastro = DateTime.Now,
                    // Avatar padrão com iniciais (temporário até adicionar imagem local)
                    FotoPerfil = "https://ui-avatars.com/api/?name=Admin+Savemoney&background=3b82f6&color=fff&size=200&bold=true"
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Alimentação", IsPredefined = true },
                new Category { Id = 2, Name = "Lazer", IsPredefined = true },
                new Category { Id = 3, Name = "Transporte", IsPredefined = true },
                new Category { Id = 4, Name = "Moradia", IsPredefined = true },
                new Category { Id = 5, Name = "Despesas Operacionais", IsPredefined = true }
            );
        }
    }
}