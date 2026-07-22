using System.Collections;
using UnityEngine;

public class MageSkeleton : Skeleton
{
	[SerializeField] private int minStacks = 1;
	[SerializeField] private int maxStacks = 3;
	[SerializeField] private int strengthPerStack = 100;
	[SerializeField] private string effectName;

	public override void Attack(Character target)
	{
		int damage = Random.Range(minAttackDamage, maxAttackDamage + 1);
        DealDamage(target, new Damage(damage, damageType, this));
        int randomStacks = Random.Range(minStacks, maxStacks + 1);
        target.ApplyStatusEffect(effectName, randomStacks + strength / strengthPerStack, this);
		AudioManager.Instance?.PlaySound(attackSound);
	}

}
