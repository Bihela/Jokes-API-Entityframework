namespace Jokes_API.Models
{
	public class Joke
	{
		public int Id { get; set; }
		public string Type { get; set; }
		public string Setup { get; set; }
		public string Punchline { get; set; }
	}
}
