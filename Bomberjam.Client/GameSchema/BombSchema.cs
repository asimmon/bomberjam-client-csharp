// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.4.44
// 


using Bomberjam.Client.Colyseus.Serializer.Schema;

namespace Bomberjam.Client.GameSchema {
	internal class BombSchema : Schema {
		[Type(0, "string")]
		public string playerId = "";

		[Type(1, "int8")]
		public int countdown = 0;

		[Type(2, "int8")]
		public int range = 0;

		[Type(3, "int8")]
		public int x = 0;

		[Type(4, "int8")]
		public int y = 0;
	}
}
