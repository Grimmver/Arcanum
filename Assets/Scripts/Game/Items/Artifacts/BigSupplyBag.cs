using UnityEngine;

public class BigSupplyBag : Item, IOnWaveSpawnItem
{
	[Header("Stat Bonus")]
	public int bonusMaxHealth = 25;

	[Header("Passive Trigger")]
	[SerializeField, Range(0, 1f)] private float healPercentageFromMaxHealth = 0.15f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		owner.MaxHealth += bonusMaxHealth;
		owner.Health += bonusMaxHealth;
	}

	public override void Unequip()
	{
		if (owner != null)
		{
			owner.MaxHealth -= bonusMaxHealth;
		}

		owner = null;
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		if (owner == null) return;

		int heal = (int)(owner.MaxHealth * healPercentageFromMaxHealth);
		owner.Heal(heal);

		ActivateAnimation();
	}
}
