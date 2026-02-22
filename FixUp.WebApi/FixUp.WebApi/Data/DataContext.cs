using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

using FixUpSolution.Models; // כדי שהוא יכיר את המחלקות מהפרויקט הראשון

namespace FixUp.WebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // כאן אנחנו מגדירים את הטבלאות שלנו
        public DbSet<Professional> Professionals { get; set; }
    }
}