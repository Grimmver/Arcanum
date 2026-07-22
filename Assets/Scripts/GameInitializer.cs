using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Manager Prefabs")]
    [SerializeField] private GameObject adManagerPrefab;
    
    private void Awake()
    {
        // Initialize AdManager if it doesn't exist
        if (AdManager.Instance == null && adManagerPrefab != null)
        {
            Instantiate(adManagerPrefab);
        }
        
        // Destroy this GameObject after initialization
        Destroy(gameObject);
    }
} 