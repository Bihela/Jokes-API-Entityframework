using Microsoft.AspNetCore.Mvc;
using global::Jokes_API.Services;

namespace Jokes_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly TokenService _tokenService;

		public AuthController(TokenService tokenService)
		{
			_tokenService = tokenService;
		}

		[HttpPost("token")]
		public IActionResult GenerateToken([FromBody] UserCredentials userCredentials)
		{
			if (userCredentials.Username == "test" && userCredentials.Password == "password")
			{
				var token = _tokenService.GenerateToken(userCredentials.Username);
				return Ok(new { Token = token });
			}
			return Unauthorized();
		}
	}

	public class UserCredentials
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
