using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Bomb
    {
        [JsonProperty(nameof(BombSchema.x))]
        public int X  { get; internal set; }

        [JsonProperty(nameof(BombSchema.y))]
        public int Y  { get; internal set; }
        
        [JsonProperty(nameof(BombSchema.playerId))]
        public string PlayerId  { get; internal set; }

        [JsonProperty(nameof(BombSchema.countdown))]
        public int Countdown  { get; internal set; }

        [JsonProperty(nameof(BombSchema.range))]
        public int Range  { get; internal set; }

        internal static Bomb CreateFromSchema(BombSchema schema)
        {
            return new Bomb().UpdateFromSchema(schema);
        }

        internal Bomb UpdateFromSchema(BombSchema schema)
        {
            this.X = schema.x;
            this.Y = schema.y;
            this.PlayerId = schema.playerId;
            this.Countdown = schema.countdown;
            this.Range = schema.range;

            return this;
        }
    }
}