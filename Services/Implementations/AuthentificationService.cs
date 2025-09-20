using BCrypt.Net;
using Data;
using Domain.DTO;
using Domain.Models;
using Org.BouncyCastle.Crypto.Generators;
using Services.Interfaces;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Services.Implementations
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly ApplicationDbContext _context;


        private readonly IJwtService _jwtService;

        public AuthentificationService(ApplicationDbContext context,IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        public ServiceResult<string> Login(LoginDTO loginDTO)
        {
            var user = _context.Readers.FirstOrDefault(u => u.Email == loginDTO.Email);
            if (user == null)
            {
                return ServiceResult<string>.Fail("Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                return ServiceResult<string>.Fail("Invalid email or password");
            }

            var token = _jwtService.GenerateToken(user);

            return ServiceResult<string>.Ok(token);
        }

        public ServiceResult<string> Register(RegisterDTO dto)
        {
            if (_context.Readers.Any(x => x.Email == dto.Email))
            {
                return ServiceResult<string>.Fail($"{dto.Email} email already exists.");
            }

            Reader reader = new Reader
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Readers.Add(reader);
            _context.SaveChanges();

            var token = _jwtService.GenerateToken(reader);

            return ServiceResult<string>.Ok(token); 
        }
    }
}
