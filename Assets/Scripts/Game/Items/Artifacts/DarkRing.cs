using UnityEngine;

public class DarkRing : Item, IOnEnemyPreTakeDamageItem
{
	[Header("Stats")]
	[SerializeField] private float removeMaxHealthFraction = 0.5f;

	[Header("Passive Trigger")]
	[SerializeField, Min(0), Tooltip("0.1f == 10%")] private float bonusDamagePercentPerActiveEffectStack = 0.1f;

	public int maxHealthRemoved;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		maxHealthRemoved = (int)(owner.MaxHealth * removeMaxHealthFraction);
		owner.MaxHealth -= maxHealthRemoved;
	}

	public override void Unequip()
	{
		if (owner != null)
		{
			owner.MaxHealth += maxHealthRemoved;
		}

		owner = null;
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (damage.amount <= 0 || enemy == null || enemy.IsDying() || damage.isBonusDamage) return;

		int totalStacks = 0;
		foreach (StatusEffect effect in enemy.StatusEffectManager.ActiveEffects)
		{
			totalStacks += effect.GetCurrentStack();
		}

		float totalBonusMultiplier = bonusDamagePercentPerActiveEffectStack * totalStacks;
		int bonusDamage = (int)(damage.amount * totalBonusMultiplier);

		enemy.TakeDamage(new Damage(bonusDamage, damage.type, damage.sender, true));

		ActivateAnimation();

		//Debug.Log($"Dealt extra {bonusDamage} damage for {totalStacks} total stacks");
	}
}
