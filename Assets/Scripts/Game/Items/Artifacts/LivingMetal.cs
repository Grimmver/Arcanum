using UnityEngine;

public class LivingMetal : Item, IOnWaveSpawnItem
{
	[Header("Passive Trigger")]
	[SerializeField][Range(0, 1)] private float armorToHealthConversionAmountPercent = 0.5f;
	[SerializeField][Min(0)] private float conversionEffectiveness = 1f;
    [HideInInspector] public int totalHealthBuff = 0;
    [HideInInspector] public int totalArmorLoss = 0;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner.MaxHealth -= totalHealthBuff;
		owner.Armor += totalArmorLoss;
		owner = null;
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		if (owner == null || owner.IsDying() || owner.IsDead()) return;

		if (owner.Armor <= 0) return;

		int armorToHealth = (int)(owner.Armor * armorToHealthConversionAmountPercent);
		int bonusHealth = (int)(armorToHealth * conversionEffectiveness);

		if (bonusHealth <= 0) return;

		totalArmorLoss += armorToHealth;
		totalHealthBuff += bonusHealth;

		owner.Armor -= armorToHealth;
		owner.MaxHealth += bonusHealth;
		owner.Health += bonusHealth;
		//Debug.Log($"{armorToHealth} armor is reduced and {bonusHealth} health is added");

		ActivateAnimation();
	}
}
