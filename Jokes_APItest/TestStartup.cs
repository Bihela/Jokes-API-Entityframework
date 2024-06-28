using JokeAPIProject.Data;
using Jokes_API.Models;
using Jokes_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Jokes_API
{
	public class TestStartup
	{
		public TestStartup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

			var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();

			services.AddControllers();
			services.AddHttpClient<JokeService>();
			services.AddDbContext<JokeContext>(options =>
				options.UseInMemoryDatabase("JokesDatabase"));

			services.AddSingleton<TokenService>();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
				};
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
