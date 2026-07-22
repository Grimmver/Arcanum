using UnityEngine;

public class GobletOfFire : Item, IOnEnemyPreTakeDamageItem
{

    [SerializeField] public string effectName;
	[SerializeField] public string decoratorPrefabName;
	[SerializeField] public string relativePrefabName;

	[Header("Passive Trigger")]
	[SerializeField][Min(0)][Tooltip("0.5f = 50%")] private float bonusDamageCoefficient = 0.25f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

        RelevantStatusEffectManager.Instance.AddDecoratorAfter(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName), RelevantStatusEffectManager.Instance.GetPrefab(relativePrefabName));
	}

	public override void Unequip()
	{
		owner = null;

        RelevantStatusEffectManager.Instance.RemoveDecorator(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName));
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (damage.amount <= 0 || enemy == null || enemy.IsDying() || damage.isBonusDamage || damage.statusEffectName != "") return;

		if (damage.type != DamageType.Fire) return;

		int bonusDamage = (int)(damage.amount * bonusDamageCoefficient);

		if (bonusDamage > 0)
		{
			//Debug.Log($"Original damage: {damage.amount}");
			damage.amount += bonusDamage;
			//Debug.Log($"Final damage: {damage.amount} which is multiplication of original damage * {bonusDamagePercentForNonPhysicalDamage} for {damage.type} damage type");
			ActivateAnimation();
		}
	}
}
