using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Reader>().HasData(
                new Reader { Id = 1, FullName = "Edgar Gasparyan", Email = "edgargasparyan10.12.2006@gmail.com", Role = "Admin", PasswordHash = "$2a$11$k9jz6nBq6Z1H1lH3yXjMe.W6kZlCwXqfXiwI4VLvVQj5a/8f/6g2e" }
            );

        }
        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer("Server=DESKTOP-V37R177\\SQLEXPRESS;Database=BookLibraryDatabase;Trusted_Connection=True;TrustServerCertificate=True");

                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
    }
}
