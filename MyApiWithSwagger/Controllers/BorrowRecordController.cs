using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Implementations;
using Services.Interfaces;
using Services.Shared;

namespace MyApiWithSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowRecordController : ControllerBase
    {
        private readonly IBorrowRecordService _borrowRecordService;

        public BorrowRecordController(IBorrowRecordService bookRecordService)
        {
            _borrowRecordService = bookRecordService;
        }


        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<BorrowRecordDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetActiveBorrows()
        {
            var activeBorrows = _borrowRecordService.GetActiveBorrows();
            if (!activeBorrows.Any())
            {
                return NotFound("There are no active records at this time.");
            }
            return Ok(activeBorrows);
        }


        [HttpGet("overdue")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<BorrowRecordDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOverdueBooks()
        {
            var activeBorrows = _borrowRecordService.GetOverdueBooks();
            if (!activeBorrows.Any())
            {
                return NotFound("There are no overdue records at this time.");
            }
            return Ok(activeBorrows);
        }


        [HttpGet("books/{bookId}/history")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<BorrowRecordDTO>), StatusCodes.Status200OK)]
        public IActionResult GetBookHistory(int bookId)
        {
            var bookHistory = _borrowRecordService.GetBookHistory(bookId).ToList();
            if (!bookHistory.Any())
            {
                return NotFound($"No history found for book with ID {bookId}");
            }
            return Ok(bookHistory);
        }

        [HttpGet("readers/{readerId}/history")]
        [Authorize(Roles = "Admin")] 
        [ProducesResponseType(typeof(List<BorrowRecordDTO>), StatusCodes.Status200OK)]
        public IActionResult GetReaderHistory(int readerId)
        {
            var readerHistory = _borrowRecordService.GetBookHistory(readerId).ToList();
            if (!readerHistory.Any())
            {
                return NotFound($"No history found for book with ID {readerId}");
            }
            return Ok(readerHistory);
        }


        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult BorrowBook(BorrowRecordDTO borrowRecordDTO)
        {
            if (borrowRecordDTO is null)
            {
                return BadRequest(new { error = "Borrow record is required." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _borrowRecordService.BorrowBook(borrowRecordDTO);

            if (!result.Success)
            {
                if (result.Message!.Contains("not found"))
                {
                    return NotFound(new { error = result.Message });
                }
                else if (result.Message!.Contains("exists"))
                {
                    return Conflict(new { error = result.Message });
                }

                return BadRequest(new { error = result.Message });
            }
            return CreatedAtAction("", result.Data);
        }

        [HttpPatch("{id}/return")]
        [Authorize(Roles = "User,Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult ReturnBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid borrow record id." });
            }

            var result = _borrowRecordService.ReturnBook(id);
            if (!result.Success)
            {
                if (result.Message!.Contains("not found"))
                    return NotFound(new { error = result.Message });

                return BadRequest(new { error = result.Message });
            }

            return Ok(new
            {
                success = true,
                message = result.Message
            });
        }
    }
}
