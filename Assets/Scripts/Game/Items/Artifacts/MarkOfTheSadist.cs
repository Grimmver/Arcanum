using UnityEngine;

public class MarkOfTheSadist : Item, IOnEnemyPreTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float bonusDamagePercentForStatusEffects = 1f;

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
		if (enemy == null || enemy.IsDying() || enemy.IsDead() || damage.amount <= 0 || damage.statusEffectName == "" || damage.isBonusDamage) return;

		int bonusDamage = Mathf.CeilToInt(damage.amount * bonusDamagePercentForStatusEffects);

		if (bonusDamage <= 0) return;

		Debug.Log(damage.amount);
		Debug.Log(bonusDamage + " " + damage.type + " " + damage.sender + " " + damage.statusEffectName + " " + true);
		enemy.TakeDamage(new Damage(bonusDamage, damage.type, damage.sender, damage.statusEffectName, true));
		//Debug.Log($"Additionally dealt {bonusDamage}");

		ActivateAnimation();
	}
}
