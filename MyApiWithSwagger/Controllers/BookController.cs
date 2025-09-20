using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;

namespace MyApiWithSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        [ProducesResponseType(typeof(List<BookDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAllBooks()
        {
            var allBooks = _bookService.GetAll();
            if (!allBooks.Any())
            {
                return NotFound("There are currently no books․");
            }
            return Ok(allBooks);
        }

        [HttpGet("available")]
        [Authorize(Roles = "User,Admin")]
        [ProducesResponseType(typeof(List<BookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetAvailableBooks()
        {
            var availableBooks = _bookService.GetAvailableBooks();
            if (!availableBooks.Any())
            {
                return NotFound("There are no books available at this time.");
            }
            return Ok(availableBooks);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetBookById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid book id." });
            }

            var bookDTO = _bookService.Get(id);
            if (bookDTO is null)
            {
                return NotFound($"Book with given id was not found.");
            }

            return Ok(bookDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult CreateBook(BookDTO bookDTO)
        {
            if (bookDTO is null)
            {
                return BadRequest(new { error = "Book is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (result, bookId) = _bookService.Add(bookDTO);
            if (!result.Success)
            {
                return Conflict(new { error = result.Message });
            }
            return CreatedAtAction(nameof(GetBookById), new { id = bookId }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateBook(int id, BookDTO bookDTO)
        {
            if (bookDTO is null)
            {
                return BadRequest(new { error = "Book is required." });
            }

            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid book id." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _bookService.Update(id, bookDTO);
            if (!result.Success)
            {
                if (result.Message!.Contains("not found"))
                    return NotFound(new { error = result.Message });

                return BadRequest(new { error = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult DeleteBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid book id." });
            }

            var result = _bookService.Delete(id);
            if (!result.Success)
            {
                if (result.Message!.Contains("not found"))
                    return NotFound(new { error = result.Message });

                return BadRequest(new { error = result.Message });
            }

            return NoContent();
        }

    }
}
