using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;

using Microsoft.EntityFrameworkCore;
using YourProjectName.Models;


namespace FixUpSolution.Data
{
    public class DataContext : DbContext,IContext
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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Message> Messages { get ; set ; }
       

        public void save()
        {
            this.SaveChanges(); 
        }

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