using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Bomb : IBomb
    {
        [JsonProperty("x")]
        public int X  { get; set; }

        [JsonProperty("y")]
        public int Y  { get; set; }
        
        [JsonProperty("playerId")]
        public string PlayerId  { get; set; }

        [JsonProperty("countdown")]
        public int Countdown  { get; set; }

        [JsonProperty("range")]
        public int Range  { get; set; }

        internal static Bomb FromSchema(BombSchema bomb)
        {
            return new Bomb
            {
                X = bomb.x,
                Y = bomb.y,
                PlayerId = bomb.playerId,
                Countdown = bomb.countdown,
                Range = bomb.range
            };
        }
    }
}