using System;
using System.Collections;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
	[Header("Enemy")]
	[SerializeField] protected LocalizedTextBlock enemyName;
	public string Name => enemyName.Get();

	[SerializeField] protected int strength = 1;
	public int Strength => strength;

	[SerializeField] protected float multiplierPerLeftStrength = 0.1f;
	public float MultiplierPerLeftStrength => multiplierPerLeftStrength;

	[SerializeField] protected int minAttackDamage = 5;
	public int MinAttackDamage => minAttackDamage;

	[SerializeField] protected int maxAttackDamage = 10;
	public int MaxAttackDamage => maxAttackDamage;

	[SerializeField] public SpriteRenderer spriteRenderer;
	public Sprite Sprite
	{
		get
		{
			if (spriteRenderer != null) return spriteRenderer.sprite;

			return GetComponentInChildren<SpriteRenderer>().sprite;

		}
	}

	[SerializeField] protected DamageType damageType = DamageType.Physical;

	[SerializeField] protected string attackSound;

	[SerializeField] protected Animator targetAnimator;

	[SerializeField] protected LocalizedTextBlock description;
	public string Description => description.Get();

	public event Action OnTurnFinished;

	protected void InvokeTurnFinished() => OnTurnFinished?.Invoke();

	protected override void Awake()
	{
		base.Awake();
		if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	public virtual void AttackPlayer()
	{
		if (Player.Instance == null) return;
		if (targetAnimator != null)
			targetAnimator.SetTrigger("Attack");

		if (statusEffectManager.IsTricked())
		{
			Attack(this);
			return;
		}

		Attack(Player.Instance);
	}

	public virtual void Attack(Character target)
	{
		int damage = Random.Range(minAttackDamage, maxAttackDamage + 1);

		DealDamage(target, new Damage(damage, damageType, this));

		AudioManager.Instance?.PlaySound(attackSound);
	}

	public virtual void BoostStatsByStrength(int strength)
	{

		float amount = Mathf.Pow(1 + multiplierPerLeftStrength, strength);

		int maxHealth = (int)(MaxHealth * amount);
		int health = (int)(Health * amount);

		SetHealth(health, maxHealth);

		Armor = (int)(Armor * amount);
		minAttackDamage = (int)(MinAttackDamage * amount);
		maxAttackDamage = (int)(MaxAttackDamage * amount);

		this.strength += strength;
	}

	public override void TakeTurn()
	{
		if (statusEffectManager.IsStunned())
			StartCoroutine(SkipTurnCoroutine());
		else
			StartCoroutine(DefaultAttackCoroutine());
	}

	private IEnumerator SkipTurnCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		OnTurnFinished?.Invoke();
	}

	private IEnumerator DefaultAttackCoroutine()
	{
		AttackPlayer();
		yield return new WaitForSeconds(0.5f);
		OnTurnFinished?.Invoke();
	}
}
