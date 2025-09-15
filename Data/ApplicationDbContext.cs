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
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "C# Basics", Author = "John Doe", ISBN = "29192-33-221211", PublishedYear = 2020,TotalCopies = 5,AvailableCopies = 4},
                new Book { Id = 2, Title = "ASP.NET Core", Author = "Jane Smith", ISBN = "3232-4442-19191", PublishedYear = 2021, TotalCopies = 6, AvailableCopies = 5 },
                new Book { Id = 3, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", ISBN = "978-0201616224", PublishedYear = 1999, TotalCopies = 10, AvailableCopies = 10 },
                new Book { Id = 4, Title = "Clean Code", Author = "Robert C. Martin", ISBN = "978-0132350884", PublishedYear = 2021, TotalCopies = 3, AvailableCopies = 3 }
            );
            modelBuilder.Entity<Reader>().HasData(
                new Reader { Id = 1, FullName = "Ani Hakobyan", Email = "ani@mail.com", PhoneNumber = "+37443993555" },
                new Reader { Id = 2, FullName = "David Sargsyan", Email = "david@mail.com", PhoneNumber = "+37498323322" },
                new Reader { Id = 3,FullName = "Edgar Gasparyan", Email = "edgargasparyan10.12.2006@gmail.com",PhoneNumber = "+37499132004"}
            );

            modelBuilder.Entity<BorrowRecord>().HasData(
                new BorrowRecord { Id = 1, BookId = 2, ReaderId = 1, BorrowDate = new DateTime(2025,9,14,18,19,30), ReturnDate = null },
                new BorrowRecord { Id = 2, BookId = 3, ReaderId = 3, BorrowDate = new DateTime(2025, 9, 14, 18, 19, 30), ReturnDate = null },
                new BorrowRecord { Id = 3, BookId = 1, ReaderId = 2, BorrowDate = new DateTime(2025, 9, 14, 18, 19, 30), ReturnDate = null }
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
