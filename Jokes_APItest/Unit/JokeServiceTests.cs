using JokeAPIProject.Data;
using Jokes_API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jokes_API.Services
{
	public class JokeService
	{
		private readonly HttpClient _httpClient;
		private readonly JokeContext _context;
		private readonly JokeApiSettings _jokeApiSettings;

		public HttpClient HttpClient { get; }
		public JokeContext Context { get; }

		public JokeService(HttpClient httpClient, JokeContext context, IOptions<JokeApiSettings> jokeApiSettings)
		{
			_httpClient = httpClient;
			_context = context;
			_jokeApiSettings = jokeApiSettings.Value;
		}

		public async Task<Joke> GetRandomJokeAsync()
		{
			var response = await _httpClient.GetStringAsync($"{_jokeApiSettings.BaseUrl}/random_joke");
			var joke = JsonConvert.DeserializeObject<Joke>(response);

			_context.Jokes.Add(joke);
			await _context.SaveChangesAsync();

			return joke;
		}

		public async Task<List<Joke>> GetTenRandomJokesAsync()
		{
			var response = await _httpClient.GetStringAsync($"{_jokeApiSettings.BaseUrl}/random_ten");
			var jokes = JsonConvert.DeserializeObject<List<Joke>>(response);

			_context.Jokes.AddRange(jokes);
			await _context.SaveChangesAsync();

			return jokes;
		}

		public async Task<Joke> GetJokeByIdAsync(int id)
		{
			var response = await _httpClient.GetStringAsync($"{_jokeApiSettings.BaseUrl}/jokes/{id}");
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
			var response = await _httpClient.GetStringAsync($"{_jokeApiSettings.BaseUrl}/jokes/{type}/random");
			var jokes = JsonConvert.DeserializeObject<List<Joke>>(response);

			var joke = jokes?.FirstOrDefault();

			if (joke != null)
			{
				_context.Jokes.Add(joke);
				await _context.SaveChangesAsync();
			}

			return joke;
		}

		public async Task<List<Joke>> GetTenJokesByTypeAsync(string type)
		{
			var response = await _httpClient.GetStringAsync($"{_jokeApiSettings.BaseUrl}/jokes/{type}/ten");
			var jokes = JsonConvert.DeserializeObject<List<Joke>>(response);

			_context.Jokes.AddRange(jokes);
			await _context.SaveChangesAsync();

			return jokes;
		}

		public async Task<bool> SubmitJokeFeedbackAsync(int jokeId, int feedbackScore)
		{
			if (feedbackScore < 1 || feedbackScore > 5)
			{
				throw new ArgumentOutOfRangeException(nameof(feedbackScore), "Feedback score must be between 1 and 5.");
			}

			var feedback = new Feedback
			{
				JokeId = jokeId,
				Score = feedbackScore
			};

			_context.Feedbacks.Add(feedback);
			await _context.SaveChangesAsync();

			return true;
		}
	}
}
