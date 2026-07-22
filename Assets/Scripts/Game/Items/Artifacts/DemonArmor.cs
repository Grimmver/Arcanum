using UnityEngine;

public class DemonArmor : Item, IOnEnemyTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float DamageToArmorMultiplication = 0.1f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnEnemyTakeDamage(Enemy enemy, Damage damage)
	{

		if (damage.amount <= 0) return;

		int armorAmount = (int)(damage.amount * DamageToArmorMultiplication);

		if (armorAmount <= 0) return;

		owner.Armor += armorAmount;

		ActivateAnimation();
	}
}
