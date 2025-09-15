using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace MyApiWithSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderController : ControllerBase
    {
        private readonly IReaderService _readerService;
        public ReaderController(IReaderService readerService)
        {
            _readerService = readerService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<ReaderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAll()
        {
            var allReaders = _readerService.GetAll();
            if (!allReaders.Any())
            {
                return NotFound("There are currently no readers․");
            }
            return Ok(allReaders);
        }

        [HttpGet("{readerId}/history")]
        [ProducesResponseType(typeof(List<BorrowRecord>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReaderHistory(int readerId)
        {
            var history = _readerService.GetAll();
            if (!history.Any())
            {
                return NotFound($"No history found for reader with ID {readerId}");
            }
            return Ok(history);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReaderDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid reader id." });
            }

            var reader = _readerService.Get(id);
            if (reader is null)
            {
                return NotFound($"Reader with id = {id} is not found.");
            }

            return Ok(reader);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReaderDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Add(ReaderDTO readerDTO)
        {
            if (readerDTO is null)
            {
                return BadRequest(new { error = "Reader is required" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (result, readerId) = _readerService.Add(readerDTO);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("not found"))
                {
                    return NotFound(new { error = result.ErrorMessage });
                }
                else if (result.ErrorMessage!.Contains("exists"))
                {
                    return Conflict(new { error = result.ErrorMessage });
                }

                return BadRequest(new { error = result.ErrorMessage });
            }

            return CreatedAtAction(nameof(Get), new { id = readerId }, result.Data);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Update(int id, ReaderDTO readerDTO)
        {
            if (readerDTO is null)
            {
                return BadRequest(new { error = "Reader is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _readerService.Update(id, readerDTO);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("not found"))
                {
                    return NotFound(new { error = result.ErrorMessage });
                }
                else if (result.ErrorMessage!.Contains("exists"))
                {
                    return Conflict(new { error = result.ErrorMessage });
                }

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
                return BadRequest(new { error = "Invalid reader id." });
            }

            var result = _readerService.Delete(id);
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
