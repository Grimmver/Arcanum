using UnityEngine;
using Utils;

public class ArmorFixingKit : Item, IOnWaveSpawnItem, IOnPlayerPreAddArmorItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int armorValue = 30;
	[SerializeField] private int armorCap = 500;
	[SerializeField] private float extraArmorCoefficient = 0.5f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		if (owner.Armor < armorValue)
		{
			owner.Armor = armorValue;
			ActivateAnimation();
		}
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		if (owner == null) return;

		if (owner.Armor < armorValue)
		{
			owner.Armor = armorValue;
			//Debug.Log($"Armor was less than {armorValue} so was fixed, Armor = {armorValue}");

			ActivateAnimation();
		}
	}

	public void OnPlayerPreAddArmor(IntRef amount)
	{
		if (amount.Value <= 0 || owner.IsDying() || owner.IsDead() || owner.Armor > armorCap) return;

		int bonusArmor = (int)(amount.Value * extraArmorCoefficient);

		if (bonusArmor > 0)
		{
			
			amount.Value += bonusArmor;

			ActivateAnimation();
		}
	}

}
