using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class BookService : IBookService
    {

        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<BookDTO> GetAll()
        {
            return _context.Books
                .Select(x => new BookDTO 
                { 
                    Title = x.Title,
                    Author = x.Author,
                    ISBN = x.ISBN,
                    PublishedYear = x.PublishedYear,
                });
        }

        public IQueryable<BookDTO> GetAvailableBooks()
        {
            return _context.Books
                .Where(x => x.AvailableCopies > 0)
                .Select(x => new BookDTO
                {
                    Title = x.Title,
                    Author = x.Author,
                    ISBN = x.ISBN,
                    PublishedYear = x.PublishedYear,
                    TotalCopies = x.TotalCopies,
                });
        }

        public BookDTO? Get(int id)
        {
            return _context.Books
                .Where(x => x.Id == id)
                .Select(x => new BookDTO
                {
                    Author = x.Author,
                    Title = x.Title,
                    ISBN = x.ISBN,
                    PublishedYear = x.PublishedYear,
                    TotalCopies = x.TotalCopies,
                })
                .SingleOrDefault();
        }
        public (ServiceResult<BookDTO>, int id) Add(BookDTO obj)
        {
            bool checkISBN = _context.Books
                .Any(x => x.ISBN == obj.ISBN);

            if (checkISBN)
            {
                return (ServiceResult<BookDTO>.Fail("A book with the same ISBN already exists."), 0);
            }

            bool checkTitleAuthor = _context.Books
                 .Any(x => x.Title == obj.Title && x.Author == obj.Author);

            if (checkTitleAuthor)
            {
                return (ServiceResult<BookDTO>.Fail("A book with the same title and author already exists."), 0);
            }

            Book book = new Book
            {
                Author = obj.Author,
                Title = obj.Title,
                ISBN = obj.ISBN,
                PublishedYear = obj.PublishedYear,
                TotalCopies = obj.TotalCopies,
                AvailableCopies = obj.TotalCopies
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            var dto = new BookDTO
            {
                Author = book.Author,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                TotalCopies = book.TotalCopies
            };

            return (ServiceResult<BookDTO>.Ok(dto), book.Id);
        }

        public ServiceResult<BookDTO> Update(int id, BookDTO obj)
        {
            var book = _context.Books.Find(id);
            if (book is null)
            {
                return ServiceResult<BookDTO>.Fail("Book with the given ID was not found");
            }

            int borrowedCount = book.TotalCopies - book.AvailableCopies;
            if (borrowedCount > obj.TotalCopies)
            {
                return ServiceResult<BookDTO>.Fail($"Cannot set TotalCopies to {obj.TotalCopies} because {borrowedCount} copies are already borrowed.");
            }

            if (_context.Books.Any(x => x.ISBN == obj.ISBN && x.Id != id))
            {
                return ServiceResult<BookDTO>.Fail("A book with the same ISBN already exists.");
            }

            if (_context.Books.Any(x => x.Title == obj.Title && x.Author == obj.Author && x.Id != id))
            {
                return ServiceResult<BookDTO>.Fail("A book with the same title and author already exists.");
            }

            book.Author = obj.Author;
            book.Title = obj.Title;
            book.PublishedYear = obj.PublishedYear;
            book.ISBN = obj.ISBN;
            book.AvailableCopies += obj.TotalCopies - book.TotalCopies;
            book.TotalCopies = obj.TotalCopies;

            _context.SaveChanges();

            var dto = new BookDTO
            {
                Author = book.Author,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                TotalCopies = book.TotalCopies
            };

            return ServiceResult<BookDTO>.Ok(dto);
        }
        public ServiceResult<bool> Delete(int id)
        {
            var book = _context.Books.Find(id);

            if (book is null)
            {
                return ServiceResult<bool>.Fail("Book with the given ID was not found");
            }

            _context.Books.Remove(book);
            _context.SaveChanges();
            return ServiceResult<bool>.Ok(true);
        }
    }
}
