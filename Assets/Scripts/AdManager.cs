using System;
using Unity.Services.LevelPlay;
using UnityEngine;
// using com.unity3d.mediation;

public class AdManager : MonoBehaviour
{
	private static AdManager instance;
	public static AdManager Instance => instance;

	[SerializeField] private GameObject adBlockerPanel;

	[Header("Ad Settings")]
	[SerializeField] private bool testMode = true;
	[SerializeField] private int defeatsRequiredForAd = 1;

	private int defeatCounter = 0;
	private bool isInterstitialReady = false;
	private Action pendingAdCallback;
	private ILevelPlayInterstitialAd interstitialAd;

	// Android and iOS app keys from LevelPlay mediation settings
	private const string androidAppKey = "5888759";
	private const string iosAppKey = "5888758";
	private const string interstitialAdUnitId = "DefaultInterstitial";

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);

		InitializeLevelPlay();
	}

	private void InitializeLevelPlay()
	{
		string appKey = "";

#if UNITY_ANDROID
		appKey = androidAppKey;
#elif UNITY_IOS
        appKey = iosAppKey;
#endif

		Debug.Log($"Initializing LevelPlay with app key: {appKey}");

		try
		{
			// Initialize LevelPlay
			LevelPlay.OnInitSuccess += OnLevelPlayInitialized;
			LevelPlay.OnInitFailed += OnLevelPlayInitializationFailed;

			if (testMode)
			{
				LevelPlay.SetMetaData("is_test_suite", "enable");
			}

			// LevelPlay.SetUserId(SystemInfo.deviceUniqueIdentifier);
			LevelPlay.Init(appKey);
		}
		catch (Exception e)
		{
			Debug.LogError($"LevelPlay initialization failed: {e.Message}");
		}
	}

	private void OnLevelPlayInitialized(LevelPlayConfiguration configuration)
	{
		Debug.Log("LevelPlay initialized successfully");
		CreateInterstitialAd();
	}

	private void OnLevelPlayInitializationFailed(LevelPlayInitError error)
	{
		Debug.LogError($"LevelPlay initialization failed: {error}");
	}

	private void CreateInterstitialAd()
	{
		try
		{
			// Create interstitial ad instance
			interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

			// Subscribe to events
			interstitialAd.OnAdLoaded += OnInterstitialAdLoaded;
			interstitialAd.OnAdLoadFailed += OnInterstitialAdLoadFailed;
			interstitialAd.OnAdDisplayed += OnInterstitialAdDisplayed;
			interstitialAd.OnAdClicked += OnInterstitialAdClicked;
			interstitialAd.OnAdClosed += OnInterstitialAdClosed;
			interstitialAd.OnAdDisplayFailed += OnInterstitialAdDisplayFailed;

			// Load first ad
			LoadInterstitial();
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to create interstitial ad: {e.Message}");
		}
	}

	private void LoadInterstitial()
	{
		if (interstitialAd != null)
		{
			try
			{
				Debug.Log("Loading interstitial ad...");
				interstitialAd.LoadAd();
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to load interstitial ad: {e.Message}");
			}
		}
	}

	public void ShowInterstitialAfterDefeat(Action onComplete)
	{
		defeatCounter++;

		if (defeatCounter >= defeatsRequiredForAd)
		{
			defeatCounter = 0;

			if (interstitialAd != null && interstitialAd.IsAdReady())
			{
				try
				{
					adBlockerPanel?.SetActive(true);
					pendingAdCallback = onComplete;
					interstitialAd.ShowAd();
				}
				catch (Exception e)
				{
					Debug.LogError($"Failed to show interstitial ad: {e.Message}");
					adBlockerPanel?.SetActive(false);
					onComplete?.Invoke();
				}
			}
			else
			{
				Debug.Log("Interstitial not ready, loading new one");
				LoadInterstitial();
				onComplete?.Invoke();
			}
		}
		else
		{
			onComplete?.Invoke();
		}
	}

	// LevelPlay Interstitial Callbacks
	private void OnInterstitialAdLoaded(LevelPlayAdInfo adInfo)
	{
		Debug.Log($"Interstitial ad loaded: {adInfo.AdUnitId}");
		isInterstitialReady = true;
	}

	private void OnInterstitialAdLoadFailed(LevelPlayAdError error)
	{
		Debug.LogError($"Interstitial ad load failed: {error.ErrorMessage}");
		isInterstitialReady = false;

		// Retry after delay
		Invoke(nameof(LoadInterstitial), 5f);
	}

	private void OnInterstitialAdDisplayed(LevelPlayAdInfo adInfo)
	{
		Debug.Log($"Interstitial ad displayed: {adInfo.AdUnitId}");
		isInterstitialReady = false;
	}

	private void OnInterstitialAdClosed(LevelPlayAdInfo adInfo)
	{
		Debug.Log($"Interstitial ad closed: {adInfo.AdUnitId}");
		adBlockerPanel?.SetActive(false);
		LoadInterstitial();
		pendingAdCallback?.Invoke();
		pendingAdCallback = null;
	}

	private void OnInterstitialAdDisplayFailed(LevelPlayAdDisplayInfoError error)
	{
		Debug.LogError($"Interstitial ad display failed: {error}");
		adBlockerPanel?.SetActive(false);
		LoadInterstitial();
		pendingAdCallback?.Invoke();
		pendingAdCallback = null;
	}

	public void ShowInterstitialAd(Action onComplete)
	{
		if (interstitialAd != null && interstitialAd.IsAdReady())
		{
			try
			{
				adBlockerPanel?.SetActive(true);
				pendingAdCallback = onComplete;
				interstitialAd.ShowAd();
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to show forced interstitial ad: {e.Message}");
				adBlockerPanel?.SetActive(false);
				onComplete?.Invoke();
			}
		}
		else
		{
			Debug.Log("Forced interstitial not ready, loading new one");
			LoadInterstitial();
			onComplete?.Invoke();
		}
	}

	private void OnInterstitialAdClicked(LevelPlayAdInfo adInfo)
	{
		Debug.Log($"Interstitial ad clicked: {adInfo.AdUnitId}");
	}

	private void OnApplicationPause(bool isPaused)
	{
		// LevelPlay handles application pause automatically
	}

	private void OnDestroy()
	{
		// Unsubscribe from LevelPlay initialization events
		LevelPlay.OnInitSuccess -= OnLevelPlayInitialized;
		LevelPlay.OnInitFailed -= OnLevelPlayInitializationFailed;

		// Clean up interstitial ad
		if (interstitialAd != null)
		{
			interstitialAd.OnAdLoaded -= OnInterstitialAdLoaded;
			interstitialAd.OnAdLoadFailed -= OnInterstitialAdLoadFailed;
			interstitialAd.OnAdDisplayed -= OnInterstitialAdDisplayed;
			interstitialAd.OnAdClicked -= OnInterstitialAdClicked;
			interstitialAd.OnAdClosed -= OnInterstitialAdClosed;
			interstitialAd.OnAdDisplayFailed -= OnInterstitialAdDisplayFailed;

			interstitialAd.DestroyAd();
			interstitialAd = null;
		}
	}
}