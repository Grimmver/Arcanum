using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class TutorialCanvas : UICanvas<TutorialCanvas>
	{
		[Header("Tutorial Canvas")]
		[SerializeField] private Tutorial[] tutorials;
		private int currentIndex = 0;

		private const string IS_FIRST_GAME_SESSION = "IsFirstGameSession";
		private const string GAME_SCENE = "Game Scene";

		private void Start()
		{
			if (SceneManager.GetActiveScene().name != GAME_SCENE) return;

			bool isFirstGameSession = PlayerPrefsEventBus.GetInt(IS_FIRST_GAME_SESSION) == 0;

			if (isFirstGameSession)
			{
				StartCoroutine(ShowTutorialAfterDelay());
				PlayerPrefsEventBus.SetInt(IS_FIRST_GAME_SESSION, 1);
			}
			else
			{
				Open(false);
			}
		}

		private IEnumerator ShowTutorialAfterDelay()
		{
			yield return null;
			Open(true);
			OpenTutorial(0);
		}

		public override void Open(bool action)
		{
			base.Open(action);

			CloseAllTutorials();
			if (action) OpenTutorial(0);
		}

		public void OpenTutorial(int index)
		{
			if (index < 0 || index >= tutorials.Length)
			{
				Open(false);
				return;
			}

			tutorials[currentIndex].gameObject.SetActive(false);

			tutorials[index].gameObject.SetActive(true);
			currentIndex = index;
		}

		public void NextTutorial() => OpenTutorial(currentIndex + 1);

		public void PreviousTutorial() => OpenTutorial(currentIndex - 1);

		private void CloseAllTutorials()
		{
			foreach (Tutorial tutorial in tutorials)
			{
				if (tutorial != null)
				{
					tutorial.gameObject.SetActive(false);
				}
			}
		}
	}
}
