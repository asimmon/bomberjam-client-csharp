using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Player : IPlayer
	{
		[JsonProperty("id")]
		public string Id  { get; set; }

		[JsonProperty("name")]
		public string Name  { get; set; }

		[JsonProperty("x")]
		public int X  { get; set; }

		[JsonProperty("y")]
		public int Y  { get; set; }

		[JsonProperty("connected")]
		public bool Connected  { get; set; }

		[JsonProperty("bombsLeft")]
		public int BombsLeft  { get; set; }

		[JsonProperty("maxBombs")]
		public int MaxBombs  { get; set; }

		[JsonProperty("bombRange")]
		public int BombRange  { get; set; }

		[JsonProperty("alive")]
		public bool Alive  { get; set; }

		[JsonProperty("respawning")]
		public int Respawning  { get; set; }

		[JsonProperty("score")]
		public int score  { get; set; }

		[JsonProperty("color")]
		public int Color  { get; set; }

		[JsonProperty("hasWon")]
		public bool HasWon  { get; set; }

		internal static Player FromSchema(PlayerSchema player)
		{
			return new Player
			{
				Id = player.id,
				Name = player.name,
				X = player.x,
				Y = player.y,
				Connected = player.connected,
				BombsLeft = player.bombsLeft,
				MaxBombs = player.maxBombs,
				BombRange = player.bombsLeft,
				Alive = player.alive,
				Respawning = player.respawning,
				score = player.score,
				Color = player.color,
				HasWon = player.hasWon
			};
		}
	}
}
