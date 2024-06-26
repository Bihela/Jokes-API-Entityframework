using JokeAPIProject.Models;
using JokeAPIProject.Services;
using Jokes_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JokeAPIProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class JokesController : ControllerBase
	{
		private readonly JokeService _jokeService;
		private readonly JokeContext _context;

		public JokesController(JokeService jokeService, JokeContext context)
		{
			_jokeService = jokeService;
			_context = context;
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
			var joke = await _context.Jokes.FindAsync(id);
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
	}
}
