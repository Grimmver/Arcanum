using UnityEngine;

public class BucketOfWater : Item, IOnEnemyPreTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float bonusIceDamageMultiplication = 0.5f;
	[SerializeField][Min(0)] private float bonusLightningDamageMultiplication = 1f;

	[SerializeField] public string effectName;
	[SerializeField] public string decoratorPrefabName;
	[SerializeField] public string relativePrefabName;

	public override void Equip(Player player)
	{
		owner = player;

		if (owner == null)
		{
			return;
		}

		RelevantStatusEffectManager.Instance.AddDecoratorAfter(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName), RelevantStatusEffectManager.Instance.GetPrefab(relativePrefabName));
	}

	public override void Unequip()
	{

		if (owner != null)
		{
			RelevantStatusEffectManager.Instance.RemoveDecorator(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName));
		}

		owner = null;
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (enemy == null || enemy.IsDying() || enemy.IsDead() || damage.amount <= 0) return;

		bool activated = false;
		if (damage.type == DamageType.Ice)
		{
			int bonusDamage = (int)(damage.amount * bonusIceDamageMultiplication);
			//Debug.Log($"Original Damage was {damage.amount} damage and {damage.type} type");
			damage.amount += bonusDamage;
			//Debug.Log($"So it was multiplied for {damage.amount} adding {bonusIceDamageMultiplication} as a bonus multiplication");

			activated = true;
		}
		if (damage.type == DamageType.Lighting)
		{
			int bonusDamage = (int)(damage.amount * bonusLightningDamageMultiplication);
			//Debug.Log($"Original Damage was {damage.amount} damage and {damage.type} type");
			damage.amount += bonusDamage;
			//Debug.Log($"So it was multiplied for {damage.amount} adding {bonusLightningDamageMultiplication} as a bonus multiplication");

			activated = true;
		}

		if (activated)
		{
			ActivateAnimation();
		}
	}
}
