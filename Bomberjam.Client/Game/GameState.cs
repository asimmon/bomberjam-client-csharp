// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.4.44
// 

using Bomberjam.Client.Colyseus.Serializer.Schema;

namespace Bomberjam.Client.Game {
	public class GameState : Schema {
		[Type(0, "string")]
		public string roomId = "";

		[Type(1, "string")]
		public string ownerId = "";

		[Type(2, "int8")]
		public int state = 0;

		[Type(3, "int32")]
		public int tick = 0;

		[Type(4, "string")]
		public string tiles = "";

		[Type(5, "map", typeof(MapSchema<Player>))]
		public MapSchema<Player> players = new MapSchema<Player>();

		[Type(6, "map", typeof(MapSchema<Bomb>))]
		public MapSchema<Bomb> bombs = new MapSchema<Bomb>();

		[Type(7, "map", typeof(MapSchema<Bonus>))]
		public MapSchema<Bonus> bonuses = new MapSchema<Bonus>();

		[Type(8, "string")]
		public string explosions = "";

		[Type(9, "int8")]
		public int width = 0;

		[Type(10, "int8")]
		public int height = 0;

		[Type(11, "int16")]
		public short tickDuration = 0;

		[Type(12, "int16")]
		public short suddenDeathCountdown = 0;

		[Type(13, "boolean")]
		public bool suddenDeathEnabled = false;

		[Type(14, "boolean")]
		public bool isSimulationPaused = false;
	}
}
