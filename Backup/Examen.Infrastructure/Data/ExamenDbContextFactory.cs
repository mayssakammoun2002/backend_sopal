using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Examen.Infrastructure.Data
{
    public class ExamenDbContextFactory : IDesignTimeDbContextFactory<ExamenDbContext>
    {
        public ExamenDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExamenDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=.;Database=ExamenDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new ExamenDbContext(optionsBuilder.Options);
        }
    }
}