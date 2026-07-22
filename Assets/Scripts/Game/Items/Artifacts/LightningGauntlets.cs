using UnityEngine;

public class LightningGauntlets : TileElementsUpdatingItem, IOnEnemyPreTakeDamageItem
{
	[SerializeField][Range(0, 1f)] private float retriggerChance = 0.5f;
	[SerializeField][Min(0)] private float retriggerDamagePercent = 0.5f;
	[SerializeField] private int bonusArmor = 100;
	[SerializeField][Min(0)] private float bonusDamageMultiplication = 1f;
	[SerializeField] private int retriggerCap = 9;

	private int currRetriggerStreak = 0;

	public override void Equip(Player player)
	{
		base.Equip(player);
		owner.AddArmor(bonusArmor);
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
    {
        if (enemy == null || enemy.IsDying() || enemy.IsDead() || damage.amount <= 0 || damage.type != DamageType.Lighting) return;

		if (!damage.isBonusDamage)
		{
			if (currRetriggerStreak < retriggerCap && Random.value <= retriggerChance)
			{
				int newDamage = (int)(damage.amount * retriggerDamagePercent);
				if (newDamage > 0)
				{
					currRetriggerStreak ++;
					owner.DealDamage(enemy, new Damage(newDamage, damage.type, damage.sender, newDamage < 1));
					ActivateAnimation();
					// Debug.Log($"{newDamage}");	
				}
			}
			else
			{
				currRetriggerStreak = 0;
			}
		}

        int bonusDamage = (int)(damage.amount * bonusDamageMultiplication);

        if (bonusDamage <= 0) return;

        damage.amount += bonusDamage;
        //Debug.Log($"Damage was buffed for {bonusDamage}, so now it is {damage.amount}");

        ActivateAnimation();
    }

}
