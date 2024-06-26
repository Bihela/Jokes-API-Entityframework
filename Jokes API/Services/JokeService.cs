using JokeAPIProject.Data;
using Jokes_API.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jokes_API.Services
{
	public class JokeService
	{
		private readonly HttpClient _httpClient;
		private readonly JokeContext _context;

		public JokeService(HttpClient httpClient, JokeContext context)
		{
			_httpClient = httpClient;
			_context = context;
		}

		public async Task<Joke> GetRandomJokeAsync()
		{
			var response = await _httpClient.GetStringAsync("https://official-joke-api.appspot.com/random_joke");
			var joke = JsonConvert.DeserializeObject<Joke>(response);

			_context.Jokes.Add(joke);
			await _context.SaveChangesAsync();

			return joke;
		}

		public async Task<List<Joke>> GetTenRandomJokesAsync()
		{
			var response = await _httpClient.GetStringAsync("https://official-joke-api.appspot.com/random_ten");
			var jokes = JsonConvert.DeserializeObject<List<Joke>>(response);

			_context.Jokes.AddRange(jokes);
			await _context.SaveChangesAsync();

			return jokes;
		}

		public async Task<Joke> GetJokeByIdAsync(int id)
		{
			var response = await _httpClient.GetStringAsync($"https://official-joke-api.appspot.com/jokes/{id}");
			var joke = JsonConvert.DeserializeObject<Joke>(response);

			if (joke != null)
			{
				var existingJoke = await _context.Jokes.FindAsync(joke.Id);
				if (existingJoke == null)
				{
					_context.Jokes.Add(joke);
					await _context.SaveChangesAsync();
				}
			}

			return joke;
		}

		public async Task<Joke> GetRandomJokeByTypeAsync(string type)
		{
			var response = await _httpClient.GetStringAsync($"https://official-joke-api.appspot.com/jokes/{type}/random");
			var joke = JsonConvert.DeserializeObject<Joke>(response);

			_context.Jokes.Add(joke);
			await _context.SaveChangesAsync();

			return joke;
		}

		public async Task<List<Joke>> GetTenJokesByTypeAsync(string type)
		{
			var response = await _httpClient.GetStringAsync($"https://official-joke-api.appspot.com/jokes/{type}/ten");
			var jokes = JsonConvert.DeserializeObject<List<Joke>>(response);

			_context.Jokes.AddRange(jokes);
			await _context.SaveChangesAsync();

			return jokes;
		}
	}
}
