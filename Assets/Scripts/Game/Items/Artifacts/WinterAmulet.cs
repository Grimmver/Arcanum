using System.Linq;
using UnityEngine;

public class WinterAmulet : TileElementsUpdatingItem, IOnEnemyPreTakeDamageItem
{
	[Header("Stats")]
	[SerializeField] private int bonusMaxHealth = 50;

	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float bonusDamagePercentToFrozen = 0.25f;

	public override void Equip(Player player)
	{
		base.Equip(player);

		owner.MaxHealth += bonusMaxHealth;
		owner.Health += bonusMaxHealth;
	}

	public override void Unequip()
	{
		base.Unequip();

		owner.MaxHealth -= bonusMaxHealth;
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (enemy == null || enemy.IsDying() || damage.amount <= 0 || damage.isBonusDamage) return;

		bool hasFrozen = enemy.StatusEffectManager.ActiveEffects.Any(effect => effect.GetBaseEffectType() == "Frozen");

		if (!hasFrozen) return;

		// Debug.Log($"Used to be {damage.amount}");
		damage.amount = (int)(damage.amount * (1f + bonusDamagePercentToFrozen));
		ActivateAnimation();

		// Debug.Log($"Additionally dealt {bonusDamagePercentToFrozen * 100f}% more damage, so it become {damage.amount}, who has frozen - {hasFrozen}");
	}
}
