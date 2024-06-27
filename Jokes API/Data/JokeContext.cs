using Microsoft.EntityFrameworkCore;
using Jokes_API.Models;

namespace JokeAPIProject.Data
{
	public class JokeContext : DbContext
	{
		public JokeContext(DbContextOptions<JokeContext> options)
			: base(options)
		{
		}

		public DbSet<Joke> Jokes { get; set; }
		public DbSet<Feedback> Feedbacks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Joke>()
				.ToTable("Joke")
				.Property(j => j.Id)
				.ValueGeneratedNever();

			modelBuilder.Entity<Feedback>()
				.ToTable("Feedback")
				.HasKey(f => f.Id);
		}
	}
}
