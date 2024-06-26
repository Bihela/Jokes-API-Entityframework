using JokeAPIProject.Models;
using Newtonsoft.Json;

namespace JokeAPIProject.Services
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
            return await _context.Jokes.FindAsync(id);
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
