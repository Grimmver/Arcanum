using System.Collections;
using UnityEngine;

public class Golem : Enemy
{
	[SerializeField] private DamageType resistenceDamageType;
    [SerializeField] private string resistenceEffectType;
    [SerializeField] private float healCoefficient;

	public override int TakeDamage(Damage damage)
	{
		if (damage.type == resistenceDamageType)
		{
            int healAmount = (int)(maxHealth * healCoefficient);
			return -Heal(healAmount);
		}
		return base.TakeDamage(damage);
	}

    public override void ApplyStatusEffect(string effectName, int stack, Character sender)
    {
        if (effectName == resistenceEffectType)
        {
            return;
        }
        base.ApplyStatusEffect(effectName, stack, sender);
    }

}
