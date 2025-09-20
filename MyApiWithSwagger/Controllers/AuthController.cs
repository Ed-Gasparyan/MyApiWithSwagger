using Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace MyApiWithSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthentificationService _authService;

        public AuthController(IAuthentificationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return BadRequest(new { errer = "Registration is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _authService.Register(registerDTO);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var result = _authService.Login(loginDTO);

            if (!result.Success)
            {
                return BadRequest(new { error = $"{result.Message}" });
            }

            return Ok(result.Data);
        }
    }
}