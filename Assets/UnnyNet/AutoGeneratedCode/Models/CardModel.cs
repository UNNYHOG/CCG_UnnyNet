using UnityEngine;
using UnnyNet.Newtonsoft.Json;

namespace UnnyNet.Models
{
#pragma warning disable 649

	public class CardModel : BaseModel
	{



		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("sprite")]
		public readonly string Sprite;

		[JsonProperty("damage")]
		public readonly int Damage;

		[JsonProperty("armor")]
		public readonly int Armor;

		[JsonProperty("mana")]
		public readonly int Mana;

	}
#pragma warning restore 649
}