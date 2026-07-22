using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class GameCanvas : UICanvas<GameCanvas>
	{
		[Header("Game")]
		[SerializeField] private TMP_Text gameStateText;
		[SerializeField] private Image gameStateImage;

		private void Update()
		{
			string language = LocalizationEventBus.CurrentLanguage;
			bool isEN = language == "EN";

			switch (BattleManager.instance.turnPhase)
			{
				case BattleManager.TurnPhase.PlayerTurn:
					gameStateText.text = isEN ? "Player Turn" : "Ход Игрока";
					gameStateImage.color = Color.white;
					break;
				case BattleManager.TurnPhase.PlayerSwaped:
					gameStateText.text = isEN ? "Matching" : "Комбинирование";
					gameStateImage.color = Color.gray;
					break;
				case BattleManager.TurnPhase.EnemyTurn:
					gameStateText.text = isEN ? "Enemy Turn" : "Ход Врага";
					gameStateImage.color = Color.black;
					break;
			}
		}

		public void OnSettingsButtonClick() => SettingsCanvas.Instance?.Open(true);

		public void OnShopButtonClick() => ShopCanvas.Instance?.Open(true);
	}
}
