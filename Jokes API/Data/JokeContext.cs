using Jokes_API.Models;
using Microsoft.EntityFrameworkCore;

namespace JokeAPIProject.Data
{
	public class JokeContext : DbContext
	{
		public JokeContext(DbContextOptions<JokeContext> options)
			: base(options)
		{
		}

		public DbSet<Joke> Jokes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Joke>()
				.Property(j => j.Id)
				.ValueGeneratedNever();
		}
	}
}
