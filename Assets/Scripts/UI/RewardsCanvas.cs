using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class RewardsCanvas : UICanvas<RewardsCanvas>
	{
		[Header("Reward Settings")]
		[SerializeField] private int rewardAmplification = 0;
		[SerializeField][Min(0)] private float rewardMultiplicationByStrength = 5;
		[SerializeField][Range(1, 10)] private float rewardRandomRange;
		[SerializeField][Min(1)] private int claimX2Multiplier = 2;
		private int finalReward;
		private const string CRYSTAL_PREF = "Crystal";

		[Header("UI")]
		[SerializeField] private TMP_Text rewardCountText;
		[SerializeField] private TMP_Text rewardHeaderText;
		[SerializeField] private Button claimButton;

		private bool hasClaimed = false;
		private const string LANGUAGE_KEY = "Language";

		public void OnClaimButtonClick()
		{
			if (hasClaimed) return;

			hasClaimed = true;
			claimButton.interactable = false;
			Time.timeScale = 1;
			PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, PlayerPrefs.GetInt(CRYSTAL_PREF) + finalReward);

			StartCoroutine(HandleRewardClaim());
		}

		public void OnClaimX2ButtonClick()
		{
			if (hasClaimed) return;

			hasClaimed = true;
			claimButton.interactable = false;
			Time.timeScale = 1;

			AdManager.Instance.ShowInterstitialAd(() =>
			{
				PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, PlayerPrefs.GetInt(CRYSTAL_PREF) + finalReward * claimX2Multiplier);
				StartCoroutine(HandleRewardClaim());
			});
		}

		private IEnumerator HandleRewardClaim()
		{
			while (BattleManager.instance.IsProcessingMatch)
			{
				yield return null;
			}

			MatchManager.Instance?.RefreshTiles();

			while (!MatchManager.Instance.IsReady() || MatchManager.Instance.CntAnimating > 0)
			{
				yield return null;
			}

			if (EnemyWaveManager.Instance != null)
			{

				if (EnemyWaveManager.Instance.TurnCounter % 5 == 4) // Inventory.Instance?.IsInventoryFull() == false
				{
					ShopCanvas.Instance?.Open(true);
				}
				else
				{
					EnemyWaveManager.Instance.CheckAndSpawnNextWave();
				}
			}

			Open(false);
		}

		public void Open(int waveStrength)
		{
			if (waveStrength < 0) return;

			Inventory.Instance?.CancelAllDraggingItems();
			Time.timeScale = 0;
			hasClaimed = false;
			claimButton.interactable = true;

			int minReward = (waveStrength + rewardAmplification) * (int)(rewardMultiplicationByStrength);
			int reward = Random.Range(minReward, (int)(minReward * rewardRandomRange));
			finalReward = reward;

			rewardCountText.text = $"+{finalReward}";
			string language = PlayerPrefsEventBus.GetString(LANGUAGE_KEY);
			bool isEN = language == "EN";
			rewardHeaderText.text = isEN
				? $"Wave #{EnemyWaveManager.Instance.TurnCounter + 1} completed!"
				: $"Волна #{EnemyWaveManager.Instance.TurnCounter + 1} завершена!";
			Open(true);
		}
	}
}
