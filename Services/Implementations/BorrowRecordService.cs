using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class BorrowRecordService : IBorrowRecordService
    {
        private readonly ApplicationDbContext _context;
        public BorrowRecordService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<BorrowRecordDTO> GetActiveBorrows()
        {
            return _context.BorrowRecords
                .Include(x => x.Book)
                .Include(x => x.Reader)
                .Where(x => x.ReturnDate == null)
                .Select(x => new BorrowRecordDTO
                {
                    ReaderId = x.ReaderId,
                    BookId = x.BookId
                });
        }

        public IQueryable<BorrowRecordDTO> GetBookHistory(int bookId)
        {
            return _context.BorrowRecords
                           .Include(x => x.Book)
                           .Include(x => x.Reader)
                           .Where(x => x.BookId == bookId)
                           .Select(x => new BorrowRecordDTO
                           {
                               ReaderId = x.ReaderId,
                               BookId = x.BookId,
                           });
        }

        public IQueryable<BorrowRecordDTO> GetReaderHistory(int readerId)
        {
            return _context.BorrowRecords
                           .Include(x => x.Book)
                           .Include(x => x.Reader)
                           .Where(x => x.ReaderId == readerId)
                           .Select(x => new BorrowRecordDTO
                           {
                               ReaderId = x.ReaderId,
                               BookId = x.BookId,
                           });
        }

        public ServiceResult<BorrowRecordDTO> BorrowBook(BorrowRecordDTO borrowRecordDTO)
        {

            Book? book = _context.Books.FirstOrDefault(x => x.Id == borrowRecordDTO.BookId);

            if (book is null)
            {
                return ServiceResult<BorrowRecordDTO>.Fail($"Book with id {borrowRecordDTO.BookId} was not found.");
            }

            if (book.AvailableCopies <= 0)
            {
                return ServiceResult<BorrowRecordDTO>.Fail("No available copies left.");
            }

            Reader? reader = _context.Readers.FirstOrDefault(x => x.Id == borrowRecordDTO.ReaderId);

            if (reader is null)
            {
                return ServiceResult<BorrowRecordDTO>.Fail($"Reader with id {borrowRecordDTO.ReaderId} was not found.");
            }


            BorrowRecord borrowRecord = new BorrowRecord
            {
                BookId = book.Id,
                ReaderId = reader.Id,
                BorrowDate = DateTime.Now,
                ReturnDate = null,
                ReturnDue = DateTime.Now.AddDays(14)
            };
            _context.BorrowRecords.Add(borrowRecord);

            book.AvailableCopies--;
            _context.SaveChanges();

            var dto = new BorrowRecordDTO
            {
                ReaderId = reader.Id,
                BookId = book.Id
            };

            return ServiceResult<BorrowRecordDTO>.Ok(dto);
        }
        public ServiceResult<bool> ReturnBook(int borrowRecordId)
        {
            var borrowRecord = _context.BorrowRecords
                .Include(x => x.Book)
                .Include(x => x.Reader)
                .FirstOrDefault(x => x.Id == borrowRecordId);

            if (borrowRecord is null)
                return ServiceResult<bool>.Fail("Borrow record with the given ID was not found.");

            if (borrowRecord.ReturnDate is not null)
                return ServiceResult<bool>.Fail("Cannot return a borrow record that has already been returned.");

            borrowRecord.ReturnDate = DateTime.Now;
            borrowRecord.Book.AvailableCopies++;

            _context.SaveChanges();

            int overdueDays = 0;
            if (borrowRecord.ReturnDate > borrowRecord.ReturnDue)
                overdueDays = (borrowRecord.ReturnDate.Value - borrowRecord.ReturnDue).Days;

            if (overdueDays > 0)
            {
                return ServiceResult<bool>.Ok(true, $"Warning! You are {overdueDays} days late returning '{borrowRecord.Book.Title}'. " +
        "Return books on time next time, or face stricter consequences!");
            }

            return ServiceResult<bool>.Ok(true, $"Good job! You returned '{borrowRecord.Book.Title}' on time. Keep it up!");
        }


        public IQueryable<BorrowRecordDTO> GetOverdueBooks()
        {
            return _context.BorrowRecords
                .Include(x => x.Book)
                .Include(x => x.Reader)
                .Where(x => x.ReturnDate == null && x.ReturnDue < DateTime.Now)
                .Select(x => new BorrowRecordDTO
                {
                    ReaderId = x.ReaderId,
                    BookId = x.BookId
                });
        }
    }
}
