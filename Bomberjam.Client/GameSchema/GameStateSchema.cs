// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.4.44
// 

using Bomberjam.Client.Colyseus.Serializer.Schema;

namespace Bomberjam.Client.GameSchema {
	internal class GameStateSchema : Schema {
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

		[Type(5, "map", typeof(MapSchema<PlayerSchema>))]
		public MapSchema<PlayerSchema> players = new MapSchema<PlayerSchema>();

		[Type(6, "map", typeof(MapSchema<BombSchema>))]
		public MapSchema<BombSchema> bombs = new MapSchema<BombSchema>();

		[Type(7, "map", typeof(MapSchema<BonusSchema>))]
		public MapSchema<BonusSchema> bonuses = new MapSchema<BonusSchema>();

		[Type(8, "int8")]
		public int width = 0;

		[Type(9, "int8")]
		public int height = 0;

		[Type(10, "int16")]
		public short tickDuration = 0;

		[Type(11, "int16")]
		public short suddenDeathCountdown = 0;

		[Type(12, "boolean")]
		public bool suddenDeathEnabled = false;

		[Type(13, "boolean")]
		public bool isSimulationPaused = false;
	}
}
