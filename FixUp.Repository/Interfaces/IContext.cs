using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixUp.Repository.Models;
namespace FixUp.Repository.Interfaces
    {
        public interface IContext
        {

            DbSet<Client> Clients { get; set; }
            DbSet<User> Users { get; set; }
            DbSet<Professional> Professionals { get; set; }
            DbSet<FixUpTask> Tasks { get; set; }
            DbSet<Review> Reviews { get; set; }
            DbSet<Category> Categories { get; set; }

           Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
            int SaveChanges();
        }
    }

