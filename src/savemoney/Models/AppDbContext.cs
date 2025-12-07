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
        public DbSet<NotificacaoUsuario> Notificacoes { get; set; } // <--- JÁ ESTÁ AQUI

        // NOVO: DbSet para Widgets
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<UserTheme> UserThemes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =================================================================
            // CORREÇÃO CRÍTICA: Configuração global para campos decimais (Dinheiro)
            // =================================================================
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
            // =================================================================

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

            // Widget → Usuario (Cascade)
            modelBuilder.Entity<Widget>()
                .HasOne(w => w.Usuario)
                .WithMany(u => u.Widgets)
                .HasForeignKey(w => w.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario → Receitas (Cascade)
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Receitas)
                .WithOne(r => r.Usuario)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario → Despesas (Cascade)
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Despesas)
                .WithOne(d => d.Usuario)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // NotificacaoUsuario → Usuario (Cascade)
            modelBuilder.Entity<NotificacaoUsuario>()
                .HasOne(nu => nu.Usuario)
                .WithMany()
                .HasForeignKey(nu => nu.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice para widgets
            modelBuilder.Entity<Widget>()
                .HasIndex(w => w.UsuarioId);

            // Seeds... (Mantidos)
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