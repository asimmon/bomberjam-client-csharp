using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameStateStep
    {
        [JsonProperty("state")]
        public GameState State { get; internal set; }

        [JsonProperty("actions")]
        public IDictionary<string, GameAction?> Actions { get; internal set; }
    }
}