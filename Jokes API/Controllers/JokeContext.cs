using Jokes_API.Models;
using Microsoft.EntityFrameworkCore;

namespace JokeAPIProject.Models
{
	public class JokeContext : DbContext
	{
		public JokeContext(DbContextOptions<JokeContext> options)
			: base(options)
		{
		}

		public DbSet<Joke> Jokes { get; set; }
	}
}
