using System;
using CleanBudget.Models;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CleanBudget.Database
{
    public class BudgetContext : DbContext
    {
        public BudgetContext()
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {   
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasOne(u => u.Account)
                        .WithOne(a => a.User)
                        .HasForeignKey<Account>(a => a.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Currency>().HasData(
                new Currency("Azerbaijani Manat", "AZN"),
                new Currency("British Pound Sterling", "GBP"),
                new Currency("Chinese Yuan", "CNY"),
                new Currency("Euro", "EUR"),
                new Currency("Georgian Lari", "GEL"),
                new Currency("Kazakhstani Tenge", "KZT"),
                new Currency("Russian Ruble", "RUB"),
                new Currency("Turkish Lira", "TRY"),
                new Currency("Ukrainian Hryvnia", "UAH"),
                new Currency("United States Dollar", "USD")
            );
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<DeductOperation> DeductOperations { get; set; }
        public DbSet<RefillOperation> RefillOperations { get; set; }
        public DbSet<TransferOperation> TransferOperations { get; set; }
    }
}
