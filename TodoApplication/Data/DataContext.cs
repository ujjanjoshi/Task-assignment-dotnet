using Microsoft.EntityFrameworkCore;
using System;
using TodoApplication.Models;

namespace TodoApplication.Data
{
    public class DataContext:DbContext

    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(p => new { p.Email })
                .IsUnique(true);
        }
    }
}
