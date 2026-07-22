using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class Tutorial : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField] private TMP_Text headerText;
		[SerializeField] private TMP_Text descriptionText;
		[SerializeField] private Image image;

		public void OnNextButtonClick() => TutorialCanvas.Instance?.NextTutorial();

		public void OnPreviousButtonClick() => TutorialCanvas.Instance?.PreviousTutorial();
	}
}
