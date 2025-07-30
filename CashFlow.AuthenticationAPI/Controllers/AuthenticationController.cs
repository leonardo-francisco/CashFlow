using CashFlow.AuthenticationAPI.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CashFlow.AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        /// <summary>
        /// Login para obter Token de autenticação.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public IResult Login([FromBody] LoginRequest login, [FromServices] IOptions<AdminCredentials> config)
        {
            var admin = config.Value;

            if (login.Email == admin.Username && login.Password == admin.Password)
            {
                var token = TokenConfiguration.GenerateToken(login.Email);
                return Results.Ok(new { token });
            }

            return Results.Unauthorized();
        }
    }
}
