using UnityEngine;

public class CursedTear : Item, IOnPlayerHealItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float damageMultiplierFromHeal = 5f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnPlayerHeal(int amount)
	{
		if (owner.IsDead() || owner.IsDying() || amount < 0) return;

		int damage = (int)(amount * damageMultiplierFromHeal);

		if (damage <= 0) return;

		EnemyWaveManager.Instance?.CurrentWaveTakeDamage(new Damage(damage, DamageType.Blood, owner, true));
		//Debug.Log($"Player healed up to {amount}, so this item did {damage} damage to current enemy");

		ActivateAnimation();
	}
}
