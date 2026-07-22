using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static SceneLoader Instance { get; private set; }

    private void Start()
    {
        // Restore only if this scene was reloaded via QuickResume
        QuickResume.TryRestore();
    }
    private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(Instance);
			return;
		}

		Instance = this;
	}

	public void LoadScene(string sceneName) => StartCoroutine(LoadSceneAsync(sceneName));

	private IEnumerator LoadSceneAsync(string sceneName)
	{
		LoadingCanvas.Instance?.Open(true);

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

		while (!asyncLoad.isDone)
		{
			float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
			LoadingCanvas.Instance?.SetProgress(progress);
			yield return null;
		}

		LoadingCanvas.Instance?.SetProgress(1f);

		yield return new WaitForSeconds(1f);
	}
}
