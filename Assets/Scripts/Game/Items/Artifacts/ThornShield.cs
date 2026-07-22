using UnityEngine;

public class ThornShield : Item, IOnPlayerDefendDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private int minArmorRequired = 1;
	[SerializeField][Min(0)] private float damageReflectionPercent = 0.5f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnPlayerDefendDamage(Damage damage)
	{
		if (owner == null || owner.IsDying() || owner.IsDead() || damage.amount <= 0 || damage.sender == null) return;

		if (owner.Armor < minArmorRequired) return;

		int reflectionDamage = (int)(damage.amount * damageReflectionPercent);

		if (reflectionDamage <= 0) return;

		damage.sender.TakeDamage(new Damage(reflectionDamage, damage.type, owner));
		//Debug.Log($"{reflectionDamage} damage was reflected to attacker");

		ActivateAnimation();
	}
}
