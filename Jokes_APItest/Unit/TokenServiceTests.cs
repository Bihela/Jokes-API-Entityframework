using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jokes_API.Models;
using Jokes_API.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace JokeServiceTests.Tests
{
	[TestFixture]
	public class TokenServiceTests
	{
		private TokenService _tokenService;
		private Mock<IOptions<JwtSettings>> _jwtSettingsMock;
		private JwtSettings _jwtSettings;

		[SetUp]
		public void Setup()
		{
			_jwtSettings = new JwtSettings
			{
				Key = "supersecretkey12345678901234567890", 
				Issuer = "testIssuer",
				Audience = "testAudience"
			};

			_jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
			_jwtSettingsMock.Setup(s => s.Value).Returns(_jwtSettings);

			_tokenService = new TokenService(_jwtSettingsMock.Object);
		}

		[Test]
		public void GenerateToken_ShouldReturnValidJwtToken()
		{
			var userId = "12345";

			var token = _tokenService.GenerateToken(userId);

			Assert.That(token, Is.Not.Null);
			Assert.That(token, Is.TypeOf<string>());

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = _jwtSettings.Issuer,
				ValidAudience = _jwtSettings.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			}, out SecurityToken validatedToken);

			Assert.That(validatedToken, Is.Not.Null);
		}

		[Test]
		public void GenerateToken_ShouldIncludeExpectedClaims()
		{
			var userId = "12345";

			var token = _tokenService.GenerateToken(userId);

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadJwtToken(token);

			Assert.That(jwtToken, Is.Not.Null);
			Assert.That(jwtToken.Claims, Is.Not.Null);
			Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, Is.EqualTo(userId));
			Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti), Is.Not.Null);
		}

		[Test]
		public void GenerateToken_ShouldHaveValidExpiry()
		{
			var userId = "12345";

			var token = _tokenService.GenerateToken(userId);

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadJwtToken(token);

			Assert.That(jwtToken, Is.Not.Null);
			Assert.That(jwtToken.ValidTo, Is.EqualTo(DateTime.UtcNow.AddMinutes(30)).Within(TimeSpan.FromSeconds(5)));
		}
	}
}
