using UnityEngine;

public class CursedSkull : Item, IOnEnemyApplyStatusEffectItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int minRequiredPassiveEffect = 4;
	[SerializeField][Min(1)] private int curseStack = 1;

	private const string CURSE = "Curse";

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnEnemyApplyStatusEffect(Enemy enemy, string effectName, int stackCount)
	{
		if (enemy == null || enemy.IsDying() || enemy.IsDead() || effectName == CURSE) return;

		if (enemy.StatusEffectManager.ActiveEffects.Count < minRequiredPassiveEffect) return;

		enemy.ApplyStatusEffect(CURSE, curseStack, owner);
		ActivateAnimation();
	}
}
