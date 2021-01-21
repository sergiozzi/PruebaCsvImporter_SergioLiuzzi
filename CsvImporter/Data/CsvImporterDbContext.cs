using Microsoft.EntityFrameworkCore;
using CsvImporter.Models;

namespace CsvImporter.Data
{
    public class CsvImporterDbContext : DbContext
    {
        private const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=AcmeCorporationDb;Trusted_Connection=True;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }       

        #region DbSets

        public DbSet<Stock> Stock { get; set; }

        #endregion        
    }
}