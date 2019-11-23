using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Player
	{
		[JsonProperty(nameof(PlayerSchema.id))]
		public string Id  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.name))]
		public string Name  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.x))]
		public int X  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.y))]
		public int Y  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.connected))]
		public bool Connected  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.bombsLeft))]
		public int BombsLeft  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.maxBombs))]
		public int MaxBombs  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.bombRange))]
		public int BombRange  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.alive))]
		public bool Alive  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.respawning))]
		public int Respawning  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.score))]
		public int score  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.color))]
		public int Color  { get; internal set; }

		[JsonProperty(nameof(PlayerSchema.hasWon))]
		public bool HasWon  { get; internal set; }

		internal static Player CreateFromSchema(PlayerSchema schema)
		{
			return new Player().UpdateFromSchema(schema);
		}
		
		internal Player UpdateFromSchema(PlayerSchema schema)
		{
			this.Id = schema.id;
			this.Name = schema.name;
			this.X = schema.x;
			this.Y = schema.y;
			this.Connected = schema.connected;
			this.BombsLeft = schema.bombsLeft;
			this.MaxBombs = schema.maxBombs;
			this.BombRange = schema.bombRange;
			this.Alive = schema.alive;
			this.Respawning = schema.respawning;
			this.score = schema.score;
			this.Color = schema.color;
			this.HasWon = schema.hasWon;
			
			return this;
		}
	}
}
