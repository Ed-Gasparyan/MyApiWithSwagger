using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class ReaderService : IReaderService
    {

        private readonly ApplicationDbContext _context;

        public ReaderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<ReaderDTO> GetAll()
        {
            return _context.Readers
                .Select(x => new ReaderDTO
                {
                    FullName = x.FullName,
                    Email = x.Email
                });
        }
        public ReaderDTO? Get(int id)
        {
            return _context.Readers
                .Where(x => x.Id == id)
                .Select(x => new ReaderDTO
                {
                    FullName = x.FullName,
                    Email = x.Email
                })
                .SingleOrDefault();
        }

        public (ServiceResult<ReaderDTO>,int id) Add(ReaderDTO obj)
        {

            bool checkEmail = _context.Readers
                .Any(x => x.Email == obj.Email);

            if (checkEmail)
            {
                return (ServiceResult<ReaderDTO>.Fail("A reader with the same email already exists."), 0);
            }

            Reader reader = new Reader
            {
                FullName = obj.FullName,
                Email = obj.Email,
            };

            _context.Readers.Add(reader);
            _context.SaveChanges();

            var dto = new ReaderDTO 
            {
                FullName = reader.FullName,
                Email = reader.Email,
            };


            return (ServiceResult<ReaderDTO>.Ok(dto),reader.Id);
        }

        public ServiceResult<ReaderDTO> Update(int id, ReaderDTO obj)
        {
            var reader = _context.Readers.Find(id);
            if (reader is null)
                return ServiceResult<ReaderDTO>.Fail("Reader with the given ID was not found");

            bool checkEmail = _context.Readers.Any(x => x.Email == obj.Email && x.Id != id);
            if (checkEmail)
                return ServiceResult<ReaderDTO>.Fail("A reader with the same email already exists.");

            reader.FullName = obj.FullName;
            reader.Email = obj.Email;

            _context.SaveChanges();

            var updatedDto = new ReaderDTO
            {
                FullName = reader.FullName,
                Email = reader.Email,
            };

            return ServiceResult<ReaderDTO>.Ok(updatedDto);
        }


        public ServiceResult<bool> Delete(int id)
        {
            var reader = _context.Readers.Find(id);

            if (reader is null)
            {
                return ServiceResult<bool>.Fail("Reader with the given ID was not found");
            }

            _context.Readers.Remove(reader);
            _context.SaveChanges();
            return ServiceResult<bool>.Ok(true);

        }

        public ReaderDTO GetProfile(int userId)
        {
            var user = _context.Readers.Find(userId)!;


            ReaderDTO readerDTO = new ReaderDTO
            {
                FullName = user.FullName,
                Email = user.Email
            };

            return readerDTO;

        }

    }
}
