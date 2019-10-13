// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.4.44
// 

using Bomberjam.Client.Colyseus.Serializer.Schema;

namespace Bomberjam.Client.Game {
	public class Player : Schema {
		[Type(0, "string")]
		public string id = "";

		[Type(1, "string")]
		public string name = "";

		[Type(2, "boolean")]
		public bool connected = false;

		[Type(3, "int8")]
		public int x = 0;

		[Type(4, "int8")]
		public int y = 0;

		[Type(5, "int8")]
		public int bombsLeft = 0;

		[Type(6, "int8")]
		public int maxBombs = 0;

		[Type(7, "int8")]
		public int bombRange = 0;

		[Type(8, "boolean")]
		public bool alive = false;

		[Type(9, "int8")]
		public int respawning = 0;

		[Type(10, "int16")]
		public short score = 0;

		[Type(11, "int32")]
		public int color = 0;

		[Type(12, "boolean")]
		public bool hasWon = false;
	}
}
