using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class SettingsCanvas : UICanvas<SettingsCanvas>
	{
		[Header("Settings")]
		private const string SOUND_PREF = "Sound";
		private const string MUSIC_PREF = "Music";

		[Header("Sound")]
		[SerializeField] private Image soundButton;
		[SerializeField] private Sprite soundOn;
		[SerializeField] private Sprite soundOff;
		[SerializeField] private Image soundProgressBarImage;
		[SerializeField] private Slider soundSlider;

		[Header("Music")]
		[SerializeField] private Image musicButton;
		[SerializeField] private Sprite musicOn;
		[SerializeField] private Sprite musicOff;
		[SerializeField] private Image musicProgressBarImage;
		[SerializeField] private Slider musicSlider;

		private void Start()
		{
			soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
			musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

			float savedSound = PlayerPrefsEventBus.GetFloat(SOUND_PREF, 1f);
			float savedMusic = PlayerPrefsEventBus.GetFloat(MUSIC_PREF, 1f);

			soundSlider.SetValueWithoutNotify(savedSound);
			musicSlider.SetValueWithoutNotify(savedMusic);

			soundProgressBarImage.fillAmount = savedSound;
			musicProgressBarImage.fillAmount = savedMusic;

			soundButton.sprite = savedSound > 0 ? soundOn : soundOff;
			musicButton.sprite = savedMusic > 0 ? musicOn : musicOff;
		}

		public override void Open(bool action)
		{
			base.Open(action);

			Time.timeScale = action ? 0 : 1f;
		}

		private void OnSoundSliderChanged(float value)
		{
			PlayerPrefsEventBus.SetFloat(SOUND_PREF, value);
			soundProgressBarImage.fillAmount = value;

			soundButton.sprite = value > 0 ? soundOn : soundOff;
		}

		private void OnMusicSliderChanged(float value)
		{
			PlayerPrefsEventBus.SetFloat(MUSIC_PREF, value);
			musicProgressBarImage.fillAmount = value;

			musicButton.sprite = value > 0 ? musicOn : musicOff;
		}

		public void OnPrivacyPolicyButtonClick() => PrivacyPolicyCanvas.Instance?.Open(true);

		public void OnSoundButtonClick() => soundSlider.value = soundSlider.value > 0 ? 0 : 1f;

		public void OnMusicButtonClick() => musicSlider.value = musicSlider.value > 0 ? 0 : 1f;

		public void OnMenuButtonClick() => SceneLoader.Instance?.LoadScene("Main Menu Scene");
	}
}
