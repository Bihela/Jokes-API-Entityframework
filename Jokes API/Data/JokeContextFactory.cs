using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace JokeAPIProject.Data
{
	public class JokeContextFactory : IDesignTimeDbContextFactory<JokeContext>
	{
		public JokeContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<JokeContext>();
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			optionsBuilder.UseSqlServer(connectionString);

			return new JokeContext(optionsBuilder.Options);
		}
	}
}
