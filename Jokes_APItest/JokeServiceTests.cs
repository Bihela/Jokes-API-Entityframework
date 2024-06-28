using Moq;
using Jokes_API.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Jokes_API.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using JokeAPIProject.Data;
using System.Collections.Generic;
using Moq.Protected;
using Microsoft.Extensions.Options;

namespace JokeServiceTests.Tests
{
	[TestFixture]
	public class JokeServiceTests : IDisposable
	{
		private Mock<HttpMessageHandler> _httpMessageHandlerMock;
		private Mock<IHttpClientFactory> _httpClientFactoryMock;
		private HttpClient _httpClient;
		private JokeService _jokeService;
		private JokeContext _context;
		private JokeApiSettings _jokeApiSettings;

		public JokeServiceTests()
		{
		}

		[SetUp]
		public void Setup()
		{
			_httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_httpClient = new HttpClient(_httpMessageHandlerMock.Object);
			_httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_httpClient);

			var options = new DbContextOptionsBuilder<JokeContext>()
				.UseInMemoryDatabase(databaseName: "JokesDatabase")
				.Options;
			_context = new JokeContext(options);

			_context.Database.EnsureDeleted();
			_context.Database.EnsureCreated();

			_jokeApiSettings = new JokeApiSettings { BaseUrl = "https://official-joke-api.appspot.com" };
			var jokeApiSettingsOptions = Options.Create(_jokeApiSettings);
			_jokeService = new JokeService(_httpClient, _context, jokeApiSettingsOptions);

			_httpMessageHandlerMock.Protected()
				.Setup("Dispose", ItExpr.IsAny<bool>());
		}

		[TearDown]
		public void TearDown()
		{
			_httpClient.Dispose();
			_context.Dispose();
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
			_context?.Dispose();
		}

		private void SetupHttpResponse(string url, string jsonResponse)
		{
			_httpMessageHandlerMock.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == url),
					ItExpr.IsAny<CancellationToken>()
				)
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(jsonResponse)
				});
		}

		[Test]
		public async Task GetRandomJokeAsync_ShouldReturnJoke()
		{
			var joke = new Joke { Id = 1, Type = "general", Setup = "Why did the chicken cross the road?", Punchline = "To get to the other side!" };
			var jokeJson = JsonConvert.SerializeObject(joke);

			SetupHttpResponse($"{_jokeApiSettings.BaseUrl}/random_joke", jokeJson);

			var result = await _jokeService.GetRandomJokeAsync();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Setup, Is.EqualTo(joke.Setup));
			Assert.That(result.Punchline, Is.EqualTo(joke.Punchline));

			var jokeInDb = await _context.Jokes.FindAsync(joke.Id);
			Assert.That(jokeInDb, Is.Not.Null);
		}

		[Test]
		public async Task GetTenRandomJokesAsync_ShouldReturnJokes()
		{
			var jokes = new List<Joke>
			{
				new Joke { Id = 1, Type = "general", Setup = "Why did the chicken cross the road?", Punchline = "To get to the other side!" },
				new Joke { Id = 2, Type = "general", Setup = "How do you organize a space party?", Punchline = "You planet!" }
			};
			var jokesJson = JsonConvert.SerializeObject(jokes);

			SetupHttpResponse($"{_jokeApiSettings.BaseUrl}/random_ten", jokesJson);

			var result = await _jokeService.GetTenRandomJokesAsync();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(2));

			var jokesInDb = await _context.Jokes.ToListAsync();
			Assert.That(jokesInDb.Count, Is.EqualTo(2));
		}

		[Test]
		public async Task GetJokeByIdAsync_ShouldReturnJoke()
		{
			var joke = new Joke { Id = 1, Type = "general", Setup = "Why did the chicken cross the road?", Punchline = "To get to the other side!" };
			var jokeJson = JsonConvert.SerializeObject(joke);

			SetupHttpResponse($"{_jokeApiSettings.BaseUrl}/jokes/{joke.Id}", jokeJson);

			var result = await _jokeService.GetJokeByIdAsync(joke.Id);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Setup, Is.EqualTo(joke.Setup));
			Assert.That(result.Punchline, Is.EqualTo(joke.Punchline));

			var jokeInDb = await _context.Jokes.FindAsync(joke.Id);
			Assert.That(jokeInDb, Is.Not.Null);
		}

		[Test]
		public async Task GetRandomJokeByTypeAsync_ShouldReturnJoke()
		{
			var joke = new Joke { Id = 1, Type = "programming", Setup = "Why do programmers prefer dark mode?", Punchline = "Because light attracts bugs!" };
			var jokeJson = JsonConvert.SerializeObject(joke);

			SetupHttpResponse($"{_jokeApiSettings.BaseUrl}/jokes/programming/random", jokeJson);

			var result = await _jokeService.GetRandomJokeByTypeAsync("programming");

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Setup, Is.EqualTo(joke.Setup));
			Assert.That(result.Punchline, Is.EqualTo(joke.Punchline));

			var jokeInDb = await _context.Jokes.FindAsync(joke.Id);
			Assert.That(jokeInDb, Is.Not.Null);
		}

		[Test]
		public async Task GetTenJokesByTypeAsync_ShouldReturnJokes()
		{
			var jokes = new List<Joke>
			{
				new Joke { Id = 1, Type = "programming", Setup = "Why do programmers prefer dark mode?", Punchline = "Because light attracts bugs!" },
				new Joke { Id = 2, Type = "programming", Setup = "Why do Java developers wear glasses?", Punchline = "Because they don't C#." }
			};
			var jokesJson = JsonConvert.SerializeObject(jokes);

			SetupHttpResponse($"{_jokeApiSettings.BaseUrl}/jokes/programming/ten", jokesJson);

			var result = await _jokeService.GetTenJokesByTypeAsync("programming");

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(2));

			var jokesInDb = await _context.Jokes.ToListAsync();
			Assert.That(jokesInDb.Count, Is.EqualTo(2));
		}

		[Test]
		public async Task SubmitJokeFeedbackAsync_ShouldReturnTrue()
		{
			var joke = new Joke { Id = 1, Type = "general", Setup = "Why did the chicken cross the road?", Punchline = "To get to the other side!" };
			_context.Jokes.Add(joke);
			await _context.SaveChangesAsync();

			var result = await _jokeService.SubmitJokeFeedbackAsync(joke.Id, 4);

			Assert.That(result, Is.True);

			var feedbackInDb = await _context.Feedbacks.FirstOrDefaultAsync(f => f.JokeId == joke.Id);
			Assert.That(feedbackInDb, Is.Not.Null);
			Assert.That(feedbackInDb.Score, Is.EqualTo(4));
		}

		[Test]
		public void SubmitJokeFeedbackAsync_ShouldThrowArgumentOutOfRangeException()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			{
				await _jokeService.SubmitJokeFeedbackAsync(1, 6);
			});

			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			{
				await _jokeService.SubmitJokeFeedbackAsync(1, 0);
			});
		}
	}
}
