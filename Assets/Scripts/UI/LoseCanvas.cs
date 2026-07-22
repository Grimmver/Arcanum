using TMPro;
using UnityEngine;

namespace UI
{
	public class LoseCanvas : UICanvas<LoseCanvas>
	{

		[Header("Crystals")]
		[SerializeField] private int defaultCrystalCount = 200;

		[Header("Lose")]
		[SerializeField] private TMP_Text loseText;
		[SerializeField] private TMP_Text coinText;

		[Header("Coin Reward")]
		[SerializeField][Min(1f)] private float coinMultiplierRange = 2f;

		private const string COIN_PREF = "Coin";
		private const string CRYSTAL_PREF = "Crystal";

		public override void Open(bool action)
		{
			base.Open(action);

            int totalStrengthDefeated = (int)EnemyWaveManager.Instance?.TotalStrengthDefeated;
			int coin = Random.Range(totalStrengthDefeated, (int)(totalStrengthDefeated * coinMultiplierRange));

			// coinText.text = coin.ToString();

			PlayerPrefsEventBus.SetInt(COIN_PREF, PlayerPrefsEventBus.GetInt(COIN_PREF) + coin);
			if (!MatchManager.Instance.ResumeLuck)
			{
                PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, defaultCrystalCount);
                //Debug.Log("Crystalls set to " + PlayerPrefsEventBus.GetInt(CRYSTAL_PREF, 0));
            }
        }

		public void OnRestartButtonClick() => SceneLoader.Instance?.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

		public void OnMenuButtonClick() => SceneLoader.Instance?.LoadScene("Main Menu Scene");

		public void OnReplayButtonClick()
		{
			if (!MatchManager.Instance.ResumeLuck)
			{
				AdManager.Instance.ShowInterstitialAd(() =>
				{
					QuickResume.WriteCurrentState();
					SceneLoader.Instance?.LoadScene(UnityEngine.SceneManagement
													.SceneManager
													.GetActiveScene().name);
				});
			} else
			{
				FinalLoseCanvas.Instance?.Open(true);
            }
        }
	}
}
