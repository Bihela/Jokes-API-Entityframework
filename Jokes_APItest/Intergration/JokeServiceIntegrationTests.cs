using System.Net.Http;
using System.Threading.Tasks;
using JokeAPIProject.Data;
using Jokes_API;
using Jokes_API.Models;
using Jokes_API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JokeServiceTests.Tests
{
	[TestFixture]
	public class JokeServiceIntegrationTests
	{
		private TestServer _server;
		private HttpClient _client;
		private JokeContext _context;
		private JokeService _jokeService;
		private JokeApiSettings _jokeApiSettings;

		[SetUp]
		public void Setup()
		{
			var builder = new HostBuilder()
				.ConfigureWebHost(webHost =>
				{
					webHost.UseTestServer();
					webHost.UseStartup<TestStartup>(); // Use the custom TestStartup class
				});

			var host = builder.Start();

			_client = host.GetTestClient();

			var serviceProvider = host.Services;
			_context = serviceProvider.GetRequiredService<JokeContext>();
			var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
			var jokeApiSettings = Options.Create(new JokeApiSettings { BaseUrl = "https://official-joke-api.appspot.com" });

			_jokeService = new JokeService(httpClientFactory.CreateClient(), _context, jokeApiSettings);

			_jokeApiSettings = jokeApiSettings.Value;

			_context.Database.EnsureDeleted();
			_context.Database.EnsureCreated();
		}

		[TearDown]
		public void TearDown()
		{
			_client.Dispose();
			_context.Dispose();
		}

		[Test]
		public async Task GetRandomJokeAsync_ShouldReturnAndStoreJoke()
		{
			var joke = await _jokeService.GetRandomJokeAsync();

			Assert.That(joke, Is.Not.Null);
			Assert.That(joke.Setup, Is.Not.Empty);
			Assert.That(joke.Punchline, Is.Not.Empty);

			var jokeInDb = await _context.Jokes.FindAsync(joke.Id);
			Assert.That(jokeInDb, Is.Not.Null);
		}

		[Test]
		public async Task GetTenRandomJokesAsync_ShouldReturnAndStoreJokes()
		{
			var jokes = await _jokeService.GetTenRandomJokesAsync();

			Assert.That(jokes, Is.Not.Null);
			Assert.That(jokes.Count, Is.EqualTo(10));

			var jokesInDb = await _context.Jokes.ToListAsync();
			Assert.That(jokesInDb.Count, Is.EqualTo(10));
		}

		[Test]
		public async Task GetJokeByIdAsync_ShouldReturnAndStoreJoke()
		{
			var joke = await _jokeService.GetRandomJokeAsync();

			var retrievedJoke = await _jokeService.GetJokeByIdAsync(joke.Id);

			Assert.That(retrievedJoke, Is.Not.Null);
			Assert.That(retrievedJoke.Id, Is.EqualTo(joke.Id));

			var jokeInDb = await _context.Jokes.FindAsync(retrievedJoke.Id);
			Assert.That(jokeInDb, Is.Not.Null);
		}

		[Test]
		public async Task GetRandomJokeByTypeAsync_ShouldReturnAndStoreJoke()
		{
			var joke = await _jokeService.GetRandomJokeByTypeAsync("general");

			Assert.That(joke, Is.Not.Null);
			Assert.That(joke.Setup, Is.Not.Empty);
			Assert.That(joke.Punchline, Is.Not.Empty);

			var jokeInDb = await _context.Jokes.FindAsync(joke.Id);
			Assert.That(jokeInDb, Is.Not.Null);
		}

		[Test]
		public async Task GetTenJokesByTypeAsync_ShouldReturnAndStoreJokes()
		{
			var jokes = await _jokeService.GetTenJokesByTypeAsync("general");

			Assert.That(jokes, Is.Not.Null);
			Assert.That(jokes.Count, Is.EqualTo(10));

			var jokesInDb = await _context.Jokes.ToListAsync();
			Assert.That(jokesInDb.Count, Is.EqualTo(10));
		}

		[Test]
		public async Task SubmitJokeFeedbackAsync_ShouldStoreFeedback()
		{
			var joke = await _jokeService.GetRandomJokeAsync();

			var result = await _jokeService.SubmitJokeFeedbackAsync(joke.Id, 5);

			Assert.That(result, Is.True);

			var feedbackInDb = await _context.Feedbacks.FirstOrDefaultAsync(f => f.JokeId == joke.Id);
			Assert.That(feedbackInDb, Is.Not.Null);
			Assert.That(feedbackInDb.Score, Is.EqualTo(5));
		}
	}
}
