using UnityEngine;

public class MageStaff : Item, IOnEnemyPreTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)][Tooltip("0.5f = 50%")] private float bonusDamagePercentForNonPhysicalDamage = 0.5f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (damage.amount <= 0 || enemy == null || enemy.IsDying() || damage.isBonusDamage) return;

		if (damage.type == DamageType.Physical || damage.type == DamageType.Poison) return;

		int bonusDamage = (int)(damage.amount * bonusDamagePercentForNonPhysicalDamage);

		if (bonusDamage > 0)
		{
			//Debug.Log($"Original damage: {damage.amount}");
			damage.amount += bonusDamage;
			//Debug.Log($"Final damage: {damage.amount} which is multiplication of original damage * {bonusDamagePercentForNonPhysicalDamage} for {damage.type} damage type");

			ActivateAnimation();
		}
	}
}
