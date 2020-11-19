using UnityEngine;
using UnnyNet.Newtonsoft.Json;

namespace UnnyNet.Models
{
#pragma warning disable 649

	public class GameConfig : BaseModel
	{

		[JsonProperty("unnyIdPlayerConfig")]
		private string unnyIdPlayerConfig;
		private PlayerConfig playerConfig;


		[JsonIgnore]
		public PlayerConfig PlayerConfig
		{
			get
			{
				if (playerConfig == null)
					playerConfig = DataEditor.GetModelByUnnyId<PlayerConfig>(unnyIdPlayerConfig);
				return playerConfig;
			}
		}

	}
#pragma warning restore 649
}