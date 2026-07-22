using UnityEngine;

public class DruidStaff : TileElementsUpdatingItem
{

	[Header("Stats")]
	[SerializeField] private int maxHealthToAdd = 100;

	public override void Equip(Player player)
	{
		base.Equip(player);
		owner.MaxHealth += maxHealthToAdd;
		owner.Health += maxHealthToAdd;
	}

	public override void Unequip()
	{
		if (owner != null)
		{
			owner.MaxHealth -= maxHealthToAdd;
		}
		base.Unequip();
	}

}
