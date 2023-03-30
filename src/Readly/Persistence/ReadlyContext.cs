using Microsoft.EntityFrameworkCore;
using Readly.Entities;

namespace Readly.Persistence
{
    public class ReadlyContext : DbContext
    {
        public DbSet<Account> Customers { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        public ReadlyContext(DbContextOptions<ReadlyContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccountId);
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();

                entity.HasMany(e => e.MeterReadings).WithOne().HasForeignKey("AccountId").IsRequired();
            });

            modelBuilder.Entity<MeterReading>(entity =>
            {
                entity.HasKey(e => e.MeterReadingId);
                entity.Property(e => e.MeterReadValue).IsRequired();
                entity.Property(e => e.MeterReadingDateTime).IsRequired();
            });
        }
    }
}
