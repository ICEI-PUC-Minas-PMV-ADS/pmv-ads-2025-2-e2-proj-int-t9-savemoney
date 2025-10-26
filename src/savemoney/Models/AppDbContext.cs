using Microsoft.EntityFrameworkCore;

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

        // Novos DbSets para o R9 - Metas Financeiras
        public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }
        public DbSet<Aporte> Aportes { get; set; }

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
        }
    }
}