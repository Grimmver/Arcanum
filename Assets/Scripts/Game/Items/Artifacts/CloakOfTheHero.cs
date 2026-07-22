using UnityEngine;
using Utils;

public class CloakOfTheHero : Item, IOnEnemyPreTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float bonusDamageMultiplication = 1f;
	[SerializeField][Range(0, 1f)] private float healthPercentGreaterThanThreshold = 0.9f;

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
		if (enemy == null || enemy.IsDead() || enemy.IsDying() || damage.amount <= 0) return;

		float enemyHealthPercent = (float)enemy.Health / enemy.MaxHealth;

		if (!Maths.ApproximatelyGreaterThan(enemyHealthPercent, healthPercentGreaterThanThreshold)) return;

		int bonusDamage = (int)(damage.amount * bonusDamageMultiplication);

		if (bonusDamage > 0)
		{
			//Debug.Log($"Original Damage was {damage.amount}");
			damage.amount += bonusDamage;
			//Debug.Log($"After condition met, the damage was buffed for {bonusDamageMultiplication * 100}% and become {damage.amount}");

			ActivateAnimation();
		}
	}
}
