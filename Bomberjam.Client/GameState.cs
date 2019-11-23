using System;
using System.Collections;
using System.Collections.Generic;
using Bomberjam.Client.Colyseus.Serializer.Schema;
using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GameState
	{
		[JsonProperty(nameof(GameStateSchema.roomId))]
		public string RoomId  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.ownerId))]
		public string OwnerId  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.state))]
		public int State  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.tick))]
		public int Tick  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.tiles))]
		public string Tiles  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.players))]
		public IReadOnlyDictionary<string, Player> Players  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.bombs))]
		public IReadOnlyDictionary<string, Bomb> Bombs  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.bonuses))]
		public IReadOnlyDictionary<string, Bonus> Bonuses  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.width))]
		public int Width  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.height))]
		public int Height  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.tickDuration))]
		public short TickDuration  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.suddenDeathCountdown))]
		public short SuddenDeathCountdown  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.suddenDeathEnabled))]
		public bool SuddenDeathEnabled  { get; internal set; }

		[JsonProperty(nameof(GameStateSchema.isSimulationPaused))]
		public bool IsSimulationPaused  { get; internal set; }

		internal static GameState CreateFromSchema(GameStateSchema schema)
		{
			return new GameState().UpdateFromSchema(schema);
		}

		internal GameState UpdateFromSchema(GameStateSchema schema)
		{
			this.RoomId = schema.roomId;
			this.OwnerId = schema.ownerId;
			this.State = schema.state;
			this.Tick = schema.tick;
			this.Tiles = schema.tiles;
			this.Players = CreateDictionaryFromMapSchema(schema.players, Player.CreateFromSchema);
			this.Bombs = CreateDictionaryFromMapSchema(schema.bombs, Bomb.CreateFromSchema);
			this.Bonuses = CreateDictionaryFromMapSchema(schema.bonuses, Bonus.CreateFromSchema);
			this.Width = schema.width;
			this.Height = schema.height;
			this.TickDuration = schema.tickDuration;
			this.SuddenDeathCountdown = schema.suddenDeathCountdown;
			this.SuddenDeathEnabled = schema.suddenDeathEnabled;
			this.IsSimulationPaused = schema.isSimulationPaused;
			
			return this;
		}

		private static Dictionary<string, TVal> CreateDictionaryFromMapSchema<TVal, TValSchema>(
			MapSchema<TValSchema> mapSchema,
			Func<TValSchema, TVal> valueCreator
		)
		{
			var dictionary = new Dictionary<string, TVal>(mapSchema.Count);
			
			foreach (DictionaryEntry entry in mapSchema.Items)
			{
				var key = (string)entry.Key;
				var schema = (TValSchema)entry.Value;
				dictionary[key] = valueCreator(schema);
			}

			return dictionary;
		}
	}
}
