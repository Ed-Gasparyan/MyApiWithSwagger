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
    public class BorrowRecordService : IBorrowRecordService
    {
        private readonly ApplicationDbContext _context;
        public BorrowRecordService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<BorrowRecordDTO> GetAll()
        {
            return _context.BorrowRecords
                .Select(x => new BorrowRecordDTO
                {
                    BookName = x.Book.Title,
                    ReaderName = x.Reader.FullName,
                    Email = x.Reader.Email,
                    AuthorName = x.Book.Author,
                    ReaderId = x.ReaderId,
                    BookId = x.BookId
                });
        }

        public IQueryable<BorrowRecordDTO> GetActiveBorrows()
        {
            return _context.BorrowRecords
                .Include(x => x.Book)
                .Include(x => x.Reader)
                .Where(x => x.Book.AvailableCopies > 0)
                .Select(x => new BorrowRecordDTO
                {
                    ReaderName = x.Reader.FullName,
                    BookName = x.Book.Title,
                    AuthorName = x.Book.Author,
                    Email = x.Reader.Email,
                    ReaderId = x.ReaderId,
                    BookId = x.BookId
                });
        }

        public BorrowRecordDTO? Get(int id)
        {
            return _context.BorrowRecords
                .Include(x => x.Book)
                .Include(x => x.Reader)
                .Where(x => x.Id == id)
                .Select(x => new BorrowRecordDTO
                {
                    BookName = x.Book.Title,
                    ReaderName = x.Reader.FullName,
                    Email = x.Reader.Email,
                    AuthorName = x.Book.Author,
                    ReaderId = x.ReaderId,
                    BookId = x.BookId
                })
                .FirstOrDefault();
        }

        public (ServiceResult<BorrowRecordDTO>, int id) Add(BorrowRecordDTO borrowRecordDTO)
        {

            Book? book = _context.Books.FirstOrDefault(x => x.Id == borrowRecordDTO.BookId);

            if (book is null)
            {
                return (ServiceResult<BorrowRecordDTO>.Fail($"Book with id {borrowRecordDTO.BookId} was not found."), 0);
            }

            if (book.AvailableCopies <= 0)
            {
                return (ServiceResult<BorrowRecordDTO>.Fail("No available copies left."), 0);
            }

            Reader? reader = _context.Readers.FirstOrDefault(x => x.Id == borrowRecordDTO.ReaderId);

            if (reader is null)
            {
                return (ServiceResult<BorrowRecordDTO>.Fail($"Reader with name {borrowRecordDTO.ReaderId} was not found."), 0);
            }


            BorrowRecord borrowRecord = new BorrowRecord
            {
                BookId = book.Id,
                ReaderId = reader.Id,
                BorrowDate = DateTime.Now,
                ReturnDate = null
            };
            _context.BorrowRecords.Add(borrowRecord);

            book.AvailableCopies--;
            _context.SaveChanges();

            return (ServiceResult<BorrowRecordDTO>.Ok(borrowRecordDTO), borrowRecord.Id);

        }

        public ServiceResult<BorrowRecordDTO> Update(int id, BorrowRecordDTO borrowRecordDTO)
        {
            var oldBorrowRecord = _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Reader)
                .FirstOrDefault(x => x.Id == id);
            if (oldBorrowRecord is null)
            {
                return ServiceResult<BorrowRecordDTO>.Fail("Borrow record with the given ID was not found");
            }

            if (oldBorrowRecord.ReturnDate is not null)
            {
                return ServiceResult<BorrowRecordDTO>.Fail("Cannot update a borrow record that has already been returned.");
            }

            Book? book = _context.Books.FirstOrDefault(x => x.Id == borrowRecordDTO.BookId && x.AvailableCopies > 0);

            if (book is null)
            {
                return ServiceResult<BorrowRecordDTO>.Fail($"Book with name {borrowRecordDTO.BookName} was not found.");
            }

            Reader? reader = _context.Readers.FirstOrDefault(x => x.Id == borrowRecordDTO.ReaderId);

            if (reader is null)
            {
                return ServiceResult<BorrowRecordDTO>.Fail($"Reader with name {borrowRecordDTO.ReaderName} was not found.");
            }

            oldBorrowRecord.ReaderId = reader.Id;
            oldBorrowRecord.BookId = book.Id;

            _context.SaveChanges();

            return ServiceResult<BorrowRecordDTO>.Ok(borrowRecordDTO);

        }

        public ServiceResult<bool> Delete(int id)
        {
            var obj = _context.BorrowRecords.Find(id);
            if (obj == null)
            {
                return ServiceResult<bool>.Fail("Borrow record with the given ID was not found");
            }

            _context.BorrowRecords.Remove(obj);
            _context.SaveChanges();
            return ServiceResult<bool>.Ok(true);
        }

        public ServiceResult<bool> ReturnBook(int borrowRecordId)
        {
            var borrowRecord = _context.BorrowRecords
                .Include(x => x.Book)
                .Include(x => x.Reader)
                .FirstOrDefault(x => x.Id == borrowRecordId);

            if (borrowRecord is null)
            {
                return ServiceResult<bool>.Fail("Borrow record with the given ID was not found");
            }

            if (borrowRecord.ReturnDate is not null)
            {
                return ServiceResult<bool>.Fail("Cannot update a borrow record that has already been returned");
            }

            borrowRecord.ReturnDate = DateTime.Now;
            var book = _context.Books.FirstOrDefault(x => x.Id == borrowRecord.BookId)!;
            book.AvailableCopies++;
            _context.SaveChanges();

            return ServiceResult<bool>.Ok(true);
        }

    }
}
