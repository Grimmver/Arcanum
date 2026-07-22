using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

[Serializable]
public class ResistenceStat
{
	public float resistenceCoefficient = 1f;
	public DamageType resistenceDamageType;
}

public class Character : MonoBehaviour
{
	[Header("General")]
	[SerializeField] protected int health = 100;
	public int Health
	{
		get => health;
		set
		{
			health = value;

			ValidateHealth();
		}
	}

	[SerializeField] protected int maxHealth = 100;
	public int MaxHealth
	{
		get => maxHealth;
		set
		{
			maxHealth = value;

			ValidateHealth();
		}
	}

	[SerializeField] protected int armor = 100;
	public int Armor
	{
		get => armor;
		set
		{
			armor = value;
			ValidateArmor();
		}
	}
	[SerializeField] protected int damageReduction = 0;
	[SerializeField] protected List<ResistenceStat> resistenceStatsList = new List<ResistenceStat>();
	public IReadOnlyCollection<ResistenceStat> ResistenceStats => resistenceStatsList;
	public Dictionary<DamageType, float> resistenceStatsDict = new Dictionary<DamageType, float>();

	[Header("Death Animation")]
	[SerializeField] private float deathKickbackDistance = 0.3f;
	[SerializeField] private float deathKickbackSpeed = 3f;
	[SerializeField] private float deathKickbackHeight = 0.01f;
	[SerializeField] private float deathAnimationDuration = 0.3f;

	[Header("UI")]
	[SerializeField] protected Image healthBarImage;
	[SerializeField] protected TMP_Text currentHealthText;
	[SerializeField] protected Image armorImage;
	[SerializeField] protected TMP_Text currentArmorText;
	[SerializeField] private Vector3 statusEffectDisplayOffset = new Vector3(0, 0.5f, 0);
	[SerializeField] private Vector3 statusEffectDisplayScale = new Vector3(0.25f, 0.25f, 0.25f);

	public event Action<Damage> OnPreTakeDamage;
	public event Action<Damage> OnTakeDamage;
	public event Action<Damage> OnTakeExcessiveDamage;
	public event Action<Damage> OnDefendDamage;
	public event Action<IntRef> OnPreHeal;
	public event Action<IntRef> OnPreAddArmor;
	public event Action<int> OnHeal;
	public event Action<int> OnAddArmor;
	public event Action OnDie;
	public event Action<string, int> OnApplyStatusEffect;
	public event Action<string, IntRef> OnPreApplyStatusEffect;
	public event Action<Character, Damage> OnPreDealDamage;

	private BattleManager battleManager;
	private Vector3 originalPosition;
	private bool isShaking = false;
	private bool isDying = false;
	private bool isDead = false;

	protected StatusEffectManager statusEffectManager;
	public StatusEffectManager StatusEffectManager => statusEffectManager;

	protected virtual void Awake()
	{
		GameObject managerGO = new GameObject("StatusEffectManager");
		managerGO.transform.SetParent(transform);
		managerGO.transform.localPosition = statusEffectDisplayOffset;
		managerGO.transform.localScale = statusEffectDisplayScale;
		statusEffectManager = managerGO.AddComponent<StatusEffectManager>();
		statusEffectManager.Init(this);

		foreach (var stat in resistenceStatsList)
		{
			if (!resistenceStatsDict.ContainsKey(stat.resistenceDamageType))
			{
				resistenceStatsDict.Add(stat.resistenceDamageType, stat.resistenceCoefficient);
			}
			else
			{
				Debug.LogWarning($"Duplicate damage type: {stat.resistenceDamageType} in resistenceStatsList.");
			}
		}
	}

	public virtual void ApplyStatusEffect(string effectName, int stack, Character sender)
	{
		if (stack <= 0)
		{
			return;
		}

		IntRef stackRef = new IntRef(stack);
		OnPreApplyStatusEffect?.Invoke(effectName, stackRef);

		// Debug.Log("Character: Gonna apply " + effectName + " " + stack + " using " + statusEffectManager);
		statusEffectManager.ApplyEffect(effectName, stackRef.Value, sender);
		//Debug.Log($"{stackRef.Value} stacks are applied");

		OnApplyStatusEffect?.Invoke(effectName, stackRef.Value);
	}

	public virtual void ProcessStatusEffects(Action onComplete)
	{
		statusEffectManager.ProcessStatusEffects(onComplete);
	}

	public bool IsDying()
	{
		return isDying;
	}

	public bool IsDead()
	{
		return health <= 0 && !isDying;
	}

	protected virtual void Start()
	{
		battleManager = BattleManager.instance;

		if (healthBarImage == null)
		{
			healthBarImage = GetComponentInChildren<Image>();
		}
		if (currentHealthText == null)
		{
			currentHealthText = GetComponentInChildren<TMP_Text>();
		}

		originalPosition = Vector3.zero;

		health = Mathf.Clamp(health, 0, maxHealth);
		UpdateHealthUI();
		UpdateArmorUI();
	}

	public virtual int DealDamage(Character target, Damage damage)
	{
		if (target == null) return 0;
		if (damage.amount == 0) return 0;

		// Debug.Log(this + " wanted to attack for " + damage.amount + " damage...");

		damage.amount = (int)((float)damage.amount * statusEffectManager.GetOutgoingDamageModifier());

		// Debug.Log(this + " actually attacks for " + damage.amount + " damage (x" + statusEffectManager.GetOutgoingDamageModifier() + ").");

		OnPreDealDamage?.Invoke(target, damage);

		return target.TakeDamage(damage);
	}

	public virtual int TakeDamage(Damage sentDamage)
	{

		OnPreTakeDamage?.Invoke(sentDamage);

		if (resistenceStatsDict.ContainsKey(sentDamage.type))
		{
			sentDamage.amount = (int)(sentDamage.amount * resistenceStatsDict[sentDamage.type]);
		}

		int modifiedAmount = (int)(sentDamage.amount * statusEffectManager.GetIncomingDamageModifier() * statusEffectManager.GetIncomingTypedDamageModifier(sentDamage.type));

		modifiedAmount -= damageReduction;

		sentDamage.amount = modifiedAmount;

		statusEffectManager.ParseDamage(sentDamage);

		if (modifiedAmount < 0)
			return 0;

		int armorCoveredDamage = Math.Min(modifiedAmount, armor);
		int leftoverDamage = modifiedAmount - armorCoveredDamage;

		armor -= armorCoveredDamage;
		int before = health;
		health -= leftoverDamage;

		leftoverDamage = Math.Min(leftoverDamage, before);

		if (health < 0)
		{
			int excessiveDamage = Mathf.Abs(health);
			OnTakeExcessiveDamage?.Invoke(new Damage(excessiveDamage, sentDamage.type, sentDamage.sender, sentDamage.statusEffectName));
		}

		health = Mathf.Max(0, health);

		float shakeMagnitude = Mathf.Clamp(modifiedAmount * 0.01f, 0.1f, 1.0f);
		float shakeDuration = Mathf.Clamp(modifiedAmount * 0.002f, 0.2f, 0.8f);
		if (this != null) StartCoroutine(ShakeAnimation(shakeMagnitude, shakeDuration));

		if (armorCoveredDamage > 0)
			OnDefendDamage?.Invoke(new Damage(armorCoveredDamage, sentDamage.type, sentDamage.sender, sentDamage.statusEffectName, sentDamage.isBonusDamage));

		if (leftoverDamage > 0)
			OnTakeDamage?.Invoke(new Damage(leftoverDamage, sentDamage.type, sentDamage.sender, sentDamage.statusEffectName, sentDamage.isBonusDamage));

		if (health <= 0) Die();

		UpdateHealthUI();
		UpdateArmorUI();

		return leftoverDamage;
	}

	public virtual int TakePureDamage(Damage sentDamage)
	{

		if (sentDamage.amount < 0)
			return 0;

		health -= sentDamage.amount;

		if (health < 0)
		{
			int excessiveDamage = Mathf.Abs(health);
			OnTakeExcessiveDamage?.Invoke(new Damage(excessiveDamage, sentDamage.type, sentDamage.sender, sentDamage.statusEffectName));
		}

		health = Mathf.Max(0, health);

		float shakeMagnitude = Mathf.Clamp(sentDamage.amount * 0.01f, 0.1f, 1.0f);
		float shakeDuration = Mathf.Clamp(sentDamage.amount * 0.002f, 0.2f, 0.8f);
		if (this != null) StartCoroutine(ShakeAnimation(shakeMagnitude, shakeDuration));

		OnTakeDamage?.Invoke(new Damage(sentDamage.amount, sentDamage.type, sentDamage.sender, sentDamage.statusEffectName, sentDamage.isBonusDamage));

		if (health <= 0) Die();

		UpdateHealthUI();

		return sentDamage.amount;
	}

	public virtual int Heal(float amount)
	{
		if (amount < 0) return 0;

		IntRef heal = new IntRef((int)amount);
		OnPreHeal?.Invoke(heal);

		int previousHealth = health;
		health = Mathf.Min(health + heal.Value, maxHealth);

		int actualHealed = health - previousHealth;

		if (actualHealed > 0)
		{
			OnHeal?.Invoke(actualHealed);
		}

		UpdateHealthUI();

		return actualHealed;
	}

	public virtual int AddArmor(int amount)
	{

		if (amount < 0)
			return 0;

		IntRef toAdd = new IntRef(amount);
		OnPreAddArmor?.Invoke(toAdd);

		amount = toAdd.Value;
		armor += amount;

		OnAddArmor?.Invoke((int)amount);

		UpdateArmorUI();

		return (int)amount;
	}

	public virtual void Die()
	{
		if (this == null || !gameObject.activeInHierarchy || isDying) return;

		isDying = true;
		CancelInvoke();
		StopAllCoroutines();

		StartCoroutine(KickedBackAndVanish());
	}

	private void UpdateHealthUI()
	{
		float fill = Mathf.Clamp01((float)health / maxHealth);
		healthBarImage.fillAmount = fill;
		currentHealthText.text = health.ToString();
	}

	private void UpdateArmorUI()
	{
		if (armorImage == null || currentArmorText == null)
		{
			return;
		}
		if (armor == 0)
		{
			armorImage.enabled = false;
			currentArmorText.enabled = false;
			return;
		}
		armorImage.enabled = true;
		currentArmorText.enabled = true;
		currentArmorText.text = armor.ToString();
	}

	private void ValidateHealth()
	{
		if (health > maxHealth) health = maxHealth;
		if (health <= 0) Die();

		UpdateHealthUI();
	}

	private void ValidateArmor()
	{
		armor = Math.Max(0, armor);
		UpdateArmorUI();
	}

	private IEnumerator ShakeAnimation(float magnitude, float duration)
	{
		if (!gameObject.activeInHierarchy || this == null || isShaking)
			yield break;

		isShaking = true;
		float elapsed = 0f;

		magnitude = Mathf.Clamp(magnitude, 0f, 0.3f);
		duration = Mathf.Clamp(duration, 0.05f, 0.5f);

		while (elapsed < duration)
		{
			if (!gameObject.activeInHierarchy || this == null)
				yield break;

			float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
			float y = UnityEngine.Random.Range(-1f, 1f) * magnitude * 0.1f;

			transform.localPosition = originalPosition + new Vector3(x, y, 0);

			elapsed += Time.deltaTime;
			yield return null;
		}

		if (this != null)
			transform.localPosition = originalPosition;

		isShaking = false;
	}

	private IEnumerator KickedBackAndVanish()
	{
		float elapsed = 0f;
		Vector3 startPosition = transform.position;

		Vector3 targetPosition = startPosition + new Vector3(deathKickbackDistance, 0, 0);

		while (elapsed < deathAnimationDuration)
		{
			if (this == null || !gameObject.activeInHierarchy)
				yield break;

			float progress = elapsed / deathAnimationDuration;

			float xPos = Mathf.Lerp(startPosition.x, targetPosition.x, progress * deathKickbackSpeed);
			float yPos = startPosition.y + (deathKickbackHeight * 0.25f * (1 - 4 * (progress * 3 - 0.5f) * (progress * 3 - 0.5f)));

			transform.position = new Vector3(xPos, yPos, startPosition.z);

			SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				Color color = spriteRenderer.color;
				color.a = Mathf.Lerp(1f, 0f, progress);
				spriteRenderer.color = color;
			}

			elapsed += Time.deltaTime;
			yield return null;
		}

		OnDie?.Invoke();
		isDying = false;
		isDead = true;

		Destroy(gameObject);
	}

	public virtual void TakeTurn()
	{
		StartCoroutine(SkipTurn()); // Temp.!
	}

	public virtual IEnumerator SkipTurn()
	{
		yield return new WaitForSeconds(0.5f);
	}

	public void ClearStatusEffects()
	{
		statusEffectManager.ClearStatusEffects();
	}

	public void SetHealth(int health, int maxHealth)
	{
		MaxHealth = maxHealth;
		Health = health;
	}
}
