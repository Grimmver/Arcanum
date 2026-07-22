using UnityEngine;
using Utils;

public class ExecutionerMask : Item, IOnEnemyTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float bonusDamageMultiplication = 0.25f;
	[SerializeField] private float healthPercentLessThanThreshold = 0.5f;

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
		if (enemy == null || enemy.IsDead() || enemy.IsDying() || damage.amount <= 0 || damage.isBonusDamage) return;

		float enemyHealthPercent = (float)(enemy.Health) / enemy.MaxHealth;

		if (!Maths.ApproximatelyLessThan(enemyHealthPercent, healthPercentLessThanThreshold)) return;

		int bonusDamage = (int)(damage.amount * bonusDamageMultiplication);

		if (bonusDamage > 0)
		{
			enemy.TakeDamage(new Damage(bonusDamage, damage.type, damage.sender, true));
			//Debug.Log($"After condition met, the target took additional {bonusDamage} damage");

			ActivateAnimation();
		}
	}
}
