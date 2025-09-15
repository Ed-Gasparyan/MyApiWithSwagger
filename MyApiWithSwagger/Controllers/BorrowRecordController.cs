using Data;
using Domain.DTO;
using Domain.Models;
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
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BorrowRecordDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAll()
        {
            var allBorrowRecords = _borrowRecordService.GetAll();
            if (!allBorrowRecords.Any())
            {
                return NotFound("There are currently no records․");
            }
            return Ok(allBorrowRecords);
        }

        [HttpGet("active")]
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BorrowRecordDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid borrow record id." });
            }

            var borrowRecordDTO = _borrowRecordService.Get(id);

            if (borrowRecordDTO is null)
            {
                return NotFound();
            }

            return Ok(borrowRecordDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public IActionResult Add(BorrowRecordDTO borrowRecordDTO)
        {
            if (borrowRecordDTO is null)
            {
                return BadRequest(new { error = "Borrow record is required." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (result, borrowRecordId) = _borrowRecordService.Add(borrowRecordDTO);

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
            return CreatedAtAction(nameof(Get), new { id = borrowRecordId }, result.Data);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Update(int id, BorrowRecordDTO borrowRecordDTO)
        {
            if (borrowRecordDTO is null)
            {
                return BadRequest(new { error = "Borrow record is required" });
            }

            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid borrow record id." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _borrowRecordService.Update(id, borrowRecordDTO);

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

        [HttpPatch("{id}/return")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
                if (result.ErrorMessage!.Contains("not found"))
                    return NotFound(new { error = result.ErrorMessage });

                return BadRequest(new { error = result.ErrorMessage });
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid borrow record id." });
            }

            var result = _borrowRecordService.Delete(id);

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
