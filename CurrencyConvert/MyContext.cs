using CurrencyConvert.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvert
{
    class MyContext : DbContext
    {
        public DbSet<Currency> currencies { get; set; }
        public DbSet<Setting> setting { get; set; }
        public DbSet<Log> logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=CustomerDB.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>().ToTable("Currencies");
            modelBuilder.Entity<Setting>().ToTable("Setting");
            modelBuilder.Entity<Log>().ToTable("Logs");
        }

        public MyContext ()
        {
            Database.EnsureCreated();
        }
    }
}
