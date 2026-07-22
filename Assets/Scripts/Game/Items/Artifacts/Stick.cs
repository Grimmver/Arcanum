using UnityEngine;

public class Stick : Item, IOnEnemyPreTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int extraDamage = 8;
    [SerializeField] private DamageType damageType;

	public override void Equip(Player player)
	{
		owner = player;

		if (owner == null)
		{
			return;
		}

	}

	public override void Unequip()
	{

		if (owner != null)
		{
			owner = null;
		}

	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (enemy == null || enemy.IsDying() || enemy.IsDead() || damage.amount <= 0 || damage.statusEffectName != "" || damage.isBonusDamage) return;

        owner.DealDamage(enemy, new Damage(extraDamage, damageType, owner, true));

		ActivateAnimation();
	}
}
