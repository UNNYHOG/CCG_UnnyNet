using UnityEngine;
using UnnyNet.Newtonsoft.Json;

namespace UnnyNet.Models
{
#pragma warning disable 649

	public class PlayerConfig : BaseModel
	{



		[JsonProperty("hitPoints")]
		public readonly int HitPoints;

		[JsonProperty("startMana")]
		public readonly int StartMana;

		[JsonProperty("manaIncPerTurn")]
		public readonly int ManaIncPerTurn;

	}
#pragma warning restore 649
}