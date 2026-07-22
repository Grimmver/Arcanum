using UnityEngine;

public class HollowHeart : Item
{
	[Header("Stats")]
	[SerializeField] private int bonusEmptyHealth = 500;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		owner.MaxHealth += bonusEmptyHealth;
	}

	public override void Unequip()
	{
		if (owner == null) return;

		owner.MaxHealth -= bonusEmptyHealth;

		owner = null;
	}
}
