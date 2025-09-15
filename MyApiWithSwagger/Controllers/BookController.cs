using Data;
using Domain.DTO;
using Domain.Models;
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
        [ProducesResponseType(typeof(List<BookDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            return Ok(_bookService.GetAll().ToList());
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(List<BookDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAvailableBooks()
        {
            return Ok(_bookService.GetAvailableBooks().ToList());
        }

        [HttpGet("book/{bookId}/history")]
        [ProducesResponseType(typeof(List<ReaderDTO>), StatusCodes.Status200OK)]
        public IActionResult GetBookHistory(int bookId)
        {
            return Ok(_bookService.GetBookHistory(bookId).ToList());
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get(int id)
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Add(BookDTO bookDTO)
        {
            if (bookDTO is null)
            {
                return BadRequest(new { error = "Book is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (result,bookId) = _bookService.Add(bookDTO);

            return CreatedAtAction(nameof(Get), new { id = bookId }, result.Data);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, BookDTO bookDTO)
        {
            if (bookDTO is null)
            {
                return BadRequest(new { error = "Book is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _bookService.Update(id, bookDTO);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("not found"))
                    return NotFound(new { error = result.ErrorMessage });

                return BadRequest(new { error = result.ErrorMessage });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound("Invalid book id.");
            }

            var result = _bookService.Delete(id );
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("not found"))
                    return NotFound(new { error = result.ErrorMessage });

                return BadRequest(new { error = result.ErrorMessage });
            }

            return NoContent();
        }

    }
}
