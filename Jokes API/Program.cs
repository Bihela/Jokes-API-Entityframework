using System.Text;
using JokeAPIProject.Data;
using Jokes_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Jokes_API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JokeApiSettings>(builder.Configuration.GetSection("JokeApiSettings"));

builder.Services.AddControllers();
builder.Services.AddHttpClient<JokeService>();
builder.Services.AddDbContext<JokeContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
