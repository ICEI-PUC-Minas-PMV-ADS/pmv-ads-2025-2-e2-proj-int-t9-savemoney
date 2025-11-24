using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using savemoney.Models;

namespace savemoney.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Connection string do banco remoto
            optionsBuilder.UseSqlServer(
                "Data Source=SQL1003.site4now.net;Initial Catalog=db_ac05ba_savemoney;User ID=db_ac05ba_savemoney_admin;Password=savemoney09876;TrustServerCertificate=True"
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}