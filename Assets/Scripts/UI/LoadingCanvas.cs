using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class LoadingCanvas : UICanvas<LoadingCanvas>
	{
		[Header("Loading")]
		[SerializeField] private TMP_Text loadingText;
		[SerializeField] private Image loadingBarImage;

		public override void Open(bool action)
		{
			base.Open(action);

			SetProgress(0);
			SetText("Loading...");
		}

		public void SetProgress(float progress)
		{
			int progressText = (int)progress * 100;
			loadingBarImage.fillAmount = Mathf.Clamp01(progress);
			loadingText.text = $"{progressText}%";
		}

		public void SetText(string text) => loadingText.text = text;
	}
}
