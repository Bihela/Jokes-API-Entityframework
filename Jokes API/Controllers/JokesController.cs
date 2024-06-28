using Jokes_API.Models;
using Jokes_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JokeAPIProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class JokesController : ControllerBase
	{
		private readonly JokeService _jokeService;

		public JokesController(JokeService jokeService)
		{
			_jokeService = jokeService;
		}

		[HttpGet("random")]
		public async Task<ActionResult<Joke>> GetRandomJoke()
		{
			return await _jokeService.GetRandomJokeAsync();
		}

		[HttpGet("ten")]
		public async Task<ActionResult<List<Joke>>> GetTenRandomJokes()
		{
			return await _jokeService.GetTenRandomJokesAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Joke>> GetJokeById(int id)
		{
			var joke = await _jokeService.GetJokeByIdAsync(id);
			if (joke == null)
			{
				return NotFound();
			}
			return joke;
		}

		[HttpGet("{type}/random")]
		public async Task<ActionResult<Joke>> GetRandomJokeByType(string type)
		{
			return await _jokeService.GetRandomJokeByTypeAsync(type);
		}

		[HttpGet("{type}/ten")]
		public async Task<ActionResult<List<Joke>>> GetTenJokesByType(string type)
		{
			return await _jokeService.GetTenJokesByTypeAsync(type);
		}

		[HttpPost("feedback")]
		public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackRequest request)
		{
			try
			{
				var success = await _jokeService.SubmitJokeFeedbackAsync(request.JokeId, request.FeedbackScore);
				if (success)
				{
					return Ok();
				}
				else
				{
					return StatusCode(500, "Failed to submit feedback.");
				}
			}
			catch (ArgumentOutOfRangeException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception)
			{
				return StatusCode(500, "An unexpected error occurred while submitting feedback.");
			}
		}

		public class FeedbackRequest
		{
			public int JokeId { get; set; }
			public int FeedbackScore { get; set; }
		}
	}
}

