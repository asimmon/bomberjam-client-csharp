using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Bonus
	{
		[JsonProperty(nameof(BonusSchema.x))]
		public int X  { get; internal set; }
		
		[JsonProperty(nameof(BonusSchema.y))]
		public int Y  { get; internal set; }

		[JsonProperty(nameof(BonusSchema.type))]
		public string Type  { get; internal set; }
		
		internal static Bonus CreateFromSchema(BonusSchema schema)
		{
			return new Bonus().UpdateFromSchema(schema);
		}

		internal Bonus UpdateFromSchema(BonusSchema schema)
		{
			this.X = schema.x;
			this.Y = schema.y;
			this.Type = schema.type;

			return this;
		}
	}
}
