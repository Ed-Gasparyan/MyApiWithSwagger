using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;
using System.Net;
using System.Security.Claims;
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
        [Authorize(Roles = "User,Admin")]
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

        [HttpGet("my-history")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(List<BorrowRecord>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMyHistory()
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid or missing user id in token" });
            }

            var readerDTO = _readerService.GetProfile(userId);

            return Ok(readerDTO);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReaderDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult GetReaderById(int id)
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReaderDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateReader(ReaderDTO readerDTO)
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

            return CreatedAtAction(nameof(GetReaderById), new { id = readerId }, result.Data);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult UpdateReader(int id, ReaderDTO readerDTO)
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteReader(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid reader id." });
            }

            var result = _readerService.Delete(id);
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
