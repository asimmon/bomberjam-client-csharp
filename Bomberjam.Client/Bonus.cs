using Bomberjam.Client.GameSchema;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Bonus : IBonus
	{
		[JsonProperty("x")]
		public int X  { get; set; }
		
		[JsonProperty("y")]
		public int Y  { get; set; }

		[JsonProperty("type")]
		public string Type  { get; set; }
		
		internal static Bonus FromSchema(BonusSchema bonus)
		{
			return new Bonus
			{
				X = bonus.x,
				Y = bonus.y,
				Type = bonus.type
			};
		}
	}
}
