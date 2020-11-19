using UnnyNet.Models;
using System.Collections.Generic;

namespace UnnyNet
{
#pragma warning disable 649

	public partial class DataEditor
	{


		public static List<CardModel> CardModels { get; private set; }
		public static GameConfig GameConfig { get; private set; }

		static partial void PrepareGeneratedData() {
			var cardModelWrapper = ParseDictionary<CardModel>();
			if (cardModelWrapper == null || cardModelWrapper.List == null)
				CardModels = new List<CardModel>(0);
			else {
				CardModels = new List<CardModel>(cardModelWrapper.List.Length);
				foreach (var child in cardModelWrapper.List)
					CardModels.Add(child);
			}

			ParseDictionary<PlayerConfig>();

			var gameConfigWrapper = ParseDictionary<GameConfig>();
			if (gameConfigWrapper != null && gameConfigWrapper.List != null && gameConfigWrapper.List.Length > 0 && gameConfigWrapper.Config != null)
			{
				for (int i = 0; i < gameConfigWrapper.List.Length; i++)
				{
					if (gameConfigWrapper.List[i].UnnyId == gameConfigWrapper.Config.Selected)
					{
						GameConfig = gameConfigWrapper.List[i];
						break;
					}
				}
			}
			else
				GameConfig = null;

		}
	}
#pragma warning restore 649
}