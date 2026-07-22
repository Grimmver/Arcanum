using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private string mainMenuName = "Main Menu Scene";

    private void Start()
    {
        // Initialize AdManager if it doesn't exist
        if (AdManager.Instance == null)
        {
            GameObject adManagerObject = new GameObject("AdManager");
            adManagerObject.AddComponent<AdManager>();
        }
        
        SceneLoader.Instance.LoadScene(mainMenuName);
    }
}
