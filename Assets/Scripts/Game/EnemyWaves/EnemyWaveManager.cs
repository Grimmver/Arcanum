using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using Utils;

public class EnemyWaveManager : MonoBehaviour
{
	public static EnemyWaveManager Instance { get; private set; }

	[Header("Objects")]
	[SerializeField] private Transform[] enemyHolders;

	[Header("Waves")]
	[SerializeField] private List<EnemyWave> waves;
	[SerializeField] private List<int> strengthRequirements;

	[SerializeField] private TMP_Text waveNumberText;
	// [SerializeField] private TMP_Text waveStrengthText;

	[Header("Prefabs")]
	[SerializeField] private List<Enemy> enemyPrefabs;

	[SerializeField] private Enemy[] currentWave;
	public IReadOnlyList<Enemy> CurrentWave => currentWave;

	[Header("Others")]
	[SerializeField] private int turnCounter;

    [HideInInspector] public int PlayerHealth;
    [HideInInspector] public int PlayerArmor;
	public int crystal;
    public int TurnCounter
	{
		get => turnCounter;
		set => turnCounter = value;
	}

	[SerializeField] public int lastUsedStrength = 0;
	public int LastUsedStrength
	{
		get => lastUsedStrength;
		private set
		{
			totalStrengthDefeated += lastUsedStrength;
			lastUsedStrength = value;
		}
	}

	[SerializeField] private int totalStrengthDefeated;
	public int TotalStrengthDefeated => totalStrengthDefeated;

	private int currentWaveIndex = -1;

	public event Action OnTurnStart;
	public event Action OnTurnEnd;
	public event Action<Enemy, Damage> OnEnemyPreTakeDamage;
	public event Action<Enemy, Damage> OnEnemyTakeDamage;
	public event Action<Enemy, Damage> OnEnemyTakeExcessiveDamage;
	public event Action<Enemy, Damage> OnEnemyDefendDamage;
	public event Action<Enemy, string, int> OnEnemyApplyStatusEffect;
	public event Action<Enemy, string, IntRef> OnEnemyPreApplyStatusEffect;
	public event Action<Enemy[]> OnWaveSpawned;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		currentWave = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
		foreach (Enemy enemy in currentWave)
			enemy.OnDie += HandleEnemyDeath;

		CheckAndSpawnNextWave();
		BattleManager.instance.SetCurrentEnemies(currentWave);
	}

	private void OnEnable()
	{
		OnWaveSpawned += SubscribeCurrentEnemies;
		BattleManager.EventBus.OnEnemyTurnStart += StartTurn;
		BattleManager.EventBus.OnEnemyTurnEnd += EndTurn;
	}

	private void OnDisable()
	{
		OnWaveSpawned -= SubscribeCurrentEnemies;
		BattleManager.EventBus.OnEnemyTurnStart -= StartTurn;
		BattleManager.EventBus.OnEnemyTurnEnd -= EndTurn;
	}

	private void HandleEnemyDeath()
	{
		if (currentWave.All(e => e == null || e.IsDying()))
		{
			StartCoroutine(WaitForMatchCompletionThenReward());
		}
	}

	private IEnumerator WaitForMatchCompletionThenReward()
	{
		while (BattleManager.instance.IsProcessingMatch)
			yield return null;

		// Show interstitial ad before rewards
		if (AdManager.Instance != null)
		{
			bool adCompleted = false;
			AdManager.Instance.ShowInterstitialAfterDefeat(() => { adCompleted = true; });

			// Wait for ad to complete (either shown and closed, or skipped if not available)
			while (!adCompleted)
				yield return null;
		}

		RewardsCanvas.Instance?.Open(lastUsedStrength);
	}

	private void StartTurn()
	{
		OnTurnStart?.Invoke();
		StartCoroutine(ExecuteEnemiesSequentially());
	}

	private IEnumerator ExecuteEnemiesSequentially()
	{
		for (int i = 0; i < currentWave.Length; i++)
		{
			Enemy enemy = currentWave[i];

			if (enemy == null || enemy.IsDying() || enemy.IsDead())
			{
				Player.Instance.ClearStatusEffects(); // Failsafe.
				continue;
			}

			bool finished = false, isStatusProcessed = false;

			void MarkDone() => finished = true;

			enemy.ProcessStatusEffects(() => { isStatusProcessed = true; });
			yield return new WaitUntil(() => isStatusProcessed);

			if (enemy == null || enemy.IsDying() || enemy.IsDead())
			{
				Player.Instance.ClearStatusEffects(); // Failsafe.
				continue;
			}

			enemy.OnTurnFinished += MarkDone;
			enemy.TakeTurn();
			yield return new WaitUntil(() => finished);
			enemy.OnTurnFinished -= MarkDone;
		}

		OnTurnEnd?.Invoke();
		BattleManager.EventBus.EnemyTurnEnd();
	}

	private void EndTurn() { }

	public void CheckAndSpawnNextWave()
	{
		if (currentWave != null && currentWave.All(e => e.IsDying() || e == null))
		{
			currentWaveIndex++;
			PlayerHealth = Player.Instance.MaxHealth;
			PlayerArmor = Player.Instance.Armor;
			crystal = PlayerPrefsEventBus.GetInt("Crystal", 100);
            if (MatchManager.Instance.ResumeLuck)
			{
				MatchManager.Instance.DecreaseLuck(MatchManager.Instance.tempLuckDecreaseRate);
				MatchManager.Instance.tempLuckDuration -= 1;
            }
			if (MatchManager.Instance.tempLuckDuration == 0)
                MatchManager.Instance.ResumeLuck = false;
            if (currentWaveIndex < waves.Count)
				SpawnWave(waves[currentWaveIndex]);
			else
				SpawnRandomWaveFromPool(GetNextDynamicStrength());
		}
	}

	public void CurrentWaveTakeDamage(Damage damage)
	{
		if (currentWave == null || currentWave.Length == 0) return;

		foreach (Enemy enemy in currentWave)
		{
			if (enemy == null || enemy.IsDying() || enemy.IsDead()) continue;
			enemy.TakeDamage(damage);
			break;
		}
	}

	private int GetNextDynamicStrength() => LastUsedStrength += 1;

	private void SpawnRandomWaveFromPool(int requiredStrength)
	{
		TurnCounter++;
		string language = PlayerPrefsEventBus.GetString("Language");
		bool isEN = language == "EN";
		waveNumberText.text = isEN ? $"Wave #{TurnCounter + 1}" : $"Волна #{TurnCounter + 1}";
		ClearCurrentEnemies();

		List<Enemy> spawnedEnemies = new List<Enemy>();
		int usedStrength = 0;
		int holderIndex = 0;

		while (usedStrength < requiredStrength && holderIndex < enemyHolders.Length)
		{
			var candidates = enemyPrefabs.Where(e => e.Strength <= (requiredStrength - usedStrength)).ToList();
			if (candidates.Count == 0) break;

			Enemy prefab = candidates[UnityEngine.Random.Range(0, candidates.Count)];
			Enemy enemy = Instantiate(prefab, enemyHolders[holderIndex]);
			enemy.transform.localPosition = Vector3.zero;
			enemy.transform.localRotation = Quaternion.identity;
			enemy.transform.localScale = prefab.transform.localScale;

			enemy.OnDie += HandleEnemyDeath;
			spawnedEnemies.Add(enemy);

			usedStrength += enemy.Strength;
			holderIndex++;
		}

		if (spawnedEnemies.Count == 0 && holderIndex < enemyHolders.Length)
		{
			Enemy fallback = enemyPrefabs.OrderBy(e => e.Strength).First();

			Enemy enemy = Instantiate(fallback, enemyHolders[holderIndex]);
			enemy.transform.localPosition = Vector3.zero;
			enemy.transform.localRotation = Quaternion.identity;
			enemy.transform.localScale = fallback.transform.localScale;

			enemy.OnDie += HandleEnemyDeath;
			spawnedEnemies.Add(enemy);
			usedStrength = fallback.Strength;
		}

		currentWave = spawnedEnemies.ToArray();

		int missing = requiredStrength - usedStrength;
		int enemyCount = currentWave.Length;

		// for (int i = 0; missing > 0 && enemyCount > 0; i++)
		// {
		// 	currentWave[i % enemyCount].BoostStatsByStrength(1);
		// 	missing--;
		// }

		currentWave[0].BoostStatsByStrength(missing);

		LastUsedStrength = requiredStrength;
		BattleManager.instance.SetCurrentEnemies(currentWave);

		OnWaveSpawned?.Invoke(currentWave);
	}

	private void SpawnWave(EnemyWave wave)
	{
		TurnCounter++;
		string language = PlayerPrefsEventBus.GetString("Language");
		bool isEN = language == "EN";
		waveNumberText.text = isEN ? $"Wave #{TurnCounter + 1}" : $"Волна #{TurnCounter + 1}";
		ClearCurrentEnemies();

		List<Enemy> spawnedEnemies = new List<Enemy>();
		for (int i = 0; i < wave.Enemies.Count && i < enemyHolders.Length; i++)
		{
			Enemy prefab = wave.Enemies[i];
			Enemy enemy = Instantiate(prefab, enemyHolders[i]);
			enemy.transform.localPosition = Vector3.zero;
			enemy.transform.localRotation = Quaternion.identity;
			enemy.transform.localScale = prefab.transform.localScale;

			enemy.OnDie += HandleEnemyDeath;
			spawnedEnemies.Add(enemy);
		}

		currentWave = spawnedEnemies.ToArray();

		int actualStrength = currentWave.Sum(e => e.Strength);
		int requiredStrength = strengthRequirements[currentWaveIndex];

		int missing = requiredStrength - actualStrength;
		int enemyCount = currentWave.Length;

		// for (int i = 0; missing > 0 && enemyCount > 0; i++)
		// {
		// 	currentWave[i % enemyCount].BoostStatsByStrength(1);
		// 	missing--;
		// }

		currentWave[0].BoostStatsByStrength(missing);

		LastUsedStrength = requiredStrength;
		BattleManager.instance.SetCurrentEnemies(currentWave);

		OnWaveSpawned?.Invoke(currentWave);
	}

	private void ClearCurrentEnemies()
	{
		if (currentWave == null) return;

		foreach (Enemy enemy in currentWave)
		{
			if (enemy != null)
			{
				enemy.OnDie -= HandleEnemyDeath;
				Destroy(enemy.gameObject);
			}
		}
	}

	private void SubscribeCurrentEnemies(Enemy[] enemies)
	{
		foreach (Enemy enemy in enemies)
		{
			if (enemy == null) continue;

			enemy.OnTakeDamage -= CreateTakeDamageRelay(enemy);
			enemy.OnTakeDamage += CreateTakeDamageRelay(enemy);

			enemy.OnPreTakeDamage -= CreatePreTakeDamageRelay(enemy);
			enemy.OnPreTakeDamage += CreatePreTakeDamageRelay(enemy);

			enemy.OnTakeExcessiveDamage -= CreateTakeExcessiveDamageRelay(enemy);
			enemy.OnTakeExcessiveDamage += CreateTakeExcessiveDamageRelay(enemy);

			enemy.OnDefendDamage -= CreateDefendDamageRelay(enemy);
			enemy.OnDefendDamage += CreateDefendDamageRelay(enemy);

			enemy.OnApplyStatusEffect -= CreateApplyStatusEffectRelay(enemy);
			enemy.OnApplyStatusEffect += CreateApplyStatusEffectRelay(enemy);

			enemy.OnPreApplyStatusEffect -= CreatePreApplyStatusEffectRelay(enemy);
			enemy.OnPreApplyStatusEffect += CreatePreApplyStatusEffectRelay(enemy);
		}
	}

	private Action<Damage> CreateTakeDamageRelay(Enemy enemy) => damage => OnEnemyTakeDamage?.Invoke(enemy, damage);
	private Action<Damage> CreatePreTakeDamageRelay(Enemy enemy) => damage => OnEnemyPreTakeDamage?.Invoke(enemy, damage);
	private Action<Damage> CreateTakeExcessiveDamageRelay(Enemy enemy) => damage => OnEnemyTakeExcessiveDamage?.Invoke(enemy, damage);
	private Action<Damage> CreateDefendDamageRelay(Enemy enemy) => damage => OnEnemyDefendDamage?.Invoke(enemy, damage);
	private Action<string, int> CreateApplyStatusEffectRelay(Enemy enemy) => (effectName, stackCount) => OnEnemyApplyStatusEffect?.Invoke(enemy, effectName, stackCount);
	private Action<string, IntRef> CreatePreApplyStatusEffectRelay(Enemy enemy) => (effectName, stackCount) => OnEnemyPreApplyStatusEffect?.Invoke(enemy, effectName, stackCount);
}
