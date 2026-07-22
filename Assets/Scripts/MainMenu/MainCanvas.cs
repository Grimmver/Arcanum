using TMPro;
using UI;
using UnityEngine;

public class MainCanvas : UICanvas<MainCanvas>
{
	[Header("Main")]
	[SerializeField] private TMP_Text coinText;
	[SerializeField] private TMP_Text languageText;

	private void Start()
	{
		LocalizationEventBus.ChangeLanguage(LocalizationEventBus.CurrentLanguage);
		languageText.text = LocalizationEventBus.CurrentLanguage;
	}

	public void OnPlayButtonClick() => SceneLoader.Instance?.LoadScene("Game Scene");

	public void OnCollectionsButtonClick() => CollectionsCanvas.Instance?.Open(true);

	public void OnCaravanButtonClick() => CaravanCanvas.Instance?.Open(true);

	public void OnSettingsButtonClick() => SettingsCanvas.Instance?.Open(true);

	public void OnAchievementsButtonClick() => AchievementsCanvas.Instance?.Open(true);

	public void OnTutorialButtonClick() => TutorialCanvas.Instance?.Open(true);

	public void OnCreditsButtonClick() => CreditsCanvas.Instance?.Open(true);

	public void OnLanguageButtonClick()
	{
		string newLanguage = LocalizationEventBus.CurrentLanguage == LocalizationEventBus.DEFAULT_LANGUAGE ? LocalizationEventBus.RUSSIAN : LocalizationEventBus.ENGLISH;

		languageText.text = newLanguage;

		LocalizationEventBus.ChangeLanguage(newLanguage);
	}
}
