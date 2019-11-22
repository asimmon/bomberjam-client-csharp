using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GameState : IGame
	{
		[JsonProperty("roomId")]
		public string RoomId  { get; set; }

		[JsonProperty("ownerId")]
		public string OwnerId  { get; set; }

		[JsonProperty("state")]
		public int State  { get; set; }

		[JsonProperty("tick")]
		public int Tick  { get; set; }

		[JsonProperty("tiles")]
		public string Tiles  { get; set; }

		[JsonProperty("players")]
		public IReadOnlyDictionary<string, Player> Players  { get; set; }

		[JsonProperty("bombs")]
		public IReadOnlyDictionary<string, Bomb> Bombs  { get; set; }

		[JsonProperty("bonuses")]
		public IReadOnlyDictionary<string, Bonus> Bonuses  { get; set; }

		[JsonProperty("width")]
		public int Width  { get; set; }

		[JsonProperty("height")]
		public int Height  { get; set; }

		[JsonProperty("tickDuration")]
		public short TickDuration  { get; set; }

		[JsonProperty("suddenDeathCountdown")]
		public short SuddenDeathCountdown  { get; set; }

		[JsonProperty("suddenDeathEnabled")]
		public bool SuddenDeathEnabled  { get; set; }

		[JsonProperty("isSimulationPaused")]
		public bool IsSimulationPaused  { get; set; }

		internal static GameState FromSchema(GameStateSchema gameState)
		{
			return new GameState
			{
				RoomId = gameState.roomId,
				OwnerId = gameState.ownerId,
				State = gameState.state,
				Tick = gameState.tick,
				Tiles = gameState.tiles,
				Players = ToDictionary<string, Player>(gameState.players.Items),
				Width = gameState.width,
				Height = gameState.height,
				TickDuration = gameState.tickDuration,
				SuddenDeathCountdown = gameState.suddenDeathCountdown,
				SuddenDeathEnabled = gameState.suddenDeathEnabled,
				IsSimulationPaused = gameState.isSimulationPaused
			};
		}

		private static Dictionary<TKey, TVal> ToDictionary<TKey, TVal>(OrderedDictionary orderedDictionary)
		{
			var dictionary = new Dictionary<TKey, TVal>(orderedDictionary.Count);
			
			foreach (DictionaryEntry entry in orderedDictionary)
			{
				dictionary.Add((TKey)entry.Key, (TVal)entry.Value);
			}

			return dictionary;
		}
	}
}
