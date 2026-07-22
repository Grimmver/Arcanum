using UnityEngine;
using Utils;

public class Necronomicon : Item, IOnEnemyPreApplyStatusEffectItem, IOnEnemyPreTakeDamageItem, IOnPlayerPreApplyStatusEffectItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float bonusStatusStackMultiplier = 1.0f;
	[SerializeField][Min(0)] private float reduceDamagePercent = 0.25f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnEnemyPreApplyStatusEffect(Enemy enemy, string effectName, IntRef stackCount)
	{
		if (enemy == null || enemy.IsDying() || enemy.IsDead()) return;

		if (stackCount.Value <= 0) return;

		int bonusStacks = (int)(stackCount.Value * bonusStatusStackMultiplier);

		if (!effectName.StartsWith("Enemy"))
		{
			stackCount.Value += bonusStacks;
			//Debug.Log($"Additional {stackCount.Value} stacks are applied");
			ActivateAnimation();
		}

	}

	public void OnPlayerPreApplyStatusEffect(string effectName, IntRef stackCount)
	{
		if (Player.Instance == null || Player.Instance.IsDying() || Player.Instance.IsDead()) return;

		if (stackCount.Value <= 0) return;

		int bonusStacks = (int)(stackCount.Value * bonusStatusStackMultiplier);

		if (!effectName.StartsWith("Enemy"))
		{
			stackCount.Value += bonusStacks;
			//Debug.Log($"Additional {stackCount.Value} stacks are applied");
			ActivateAnimation();
		}
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (enemy == null || enemy.IsDying() || enemy.IsDead() || damage.statusEffectName != "" || damage.amount <= 0) return;

		int reduceDamage = (int)(damage.amount * reduceDamagePercent);

		//Debug.Log($"Original damage was {damage.amount}");
		damage.amount -= reduceDamage;
		//Debug.Log($"Reduced by {reduceDamage}, so final damage is {damage.amount}");

		ActivateAnimation();
	}
}
