using Data;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;
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
        public IActionResult GetAll()
        {
            return Ok(_readerService.GetAll().ToList());
        }

        [HttpGet("reader/{readerId}/history")]
        [ProducesResponseType(typeof(List<ReaderDTO>), StatusCodes.Status200OK)]
        public IActionResult GetReaderHistory(int readerId)
        {
            return Ok(_readerService.GetReaderHistory(readerId).ToList());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReaderDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return NotFound($"Reader with id = {id} is not found.");
            }

            var item = _readerService.Get(id);
            if (item is null)
            {
                return NotFound($"Reader with id = {id} is not found.");
            }

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReaderDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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

            var(result,readerId) = _readerService.Add(readerDTO);
            if (!result.Success)
            {
                return Conflict(new { error = result.ErrorMessage });
            }

            return CreatedAtAction(nameof(Get), new { id = readerId }, result.Data);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, ReaderDTO readerDTO)
        {
            if (readerDTO is null)
            {
                return NotFound($"Reader with id = {id} is not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _readerService.Update(id, readerDTO);
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
                return NotFound($"Reader with id = {id} is not found.");
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
