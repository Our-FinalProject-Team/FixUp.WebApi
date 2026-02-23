using FixUpSolution.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // הטבלאות שיוצרו בבסיס הנתונים:
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Professional> Professionals { get; set; }
        public DbSet<FixUpTask> Tasks { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // הגדרה לירושה (TPH) - כל המשתמשים בטבלה אחת
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Professional>("Professional");

            base.OnModelCreating(modelBuilder);
        }
    }
}