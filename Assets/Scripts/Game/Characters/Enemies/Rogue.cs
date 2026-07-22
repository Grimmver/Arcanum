using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Enemy
{
	[SerializeField] private int minStacks = 4;
	[SerializeField] private int maxStacks = 8;
	[SerializeField] private float successAttackChance = 0.25f;
	[SerializeField] private float failedAttackMultiplier = 0.1f;
	[SerializeField] private float successDodgeChance = 0.25f;
    [SerializeField] private float dodgeMultiplier = 0.25f;
	[SerializeField] private List <string> effectNames;

	public override void Attack(Character target)
	{

        int damage = Random.Range(minAttackDamage, maxAttackDamage + 1);
        bool canAttack = (UnityEngine.Random.value < successAttackChance);

        if (!canAttack)
        {
            damage = (int)(damage * failedAttackMultiplier);
        }
        else
        {
            foreach (string effectName in effectNames)
            {
                int randomStacks = Random.Range(minStacks, maxStacks + 1);
                target.ApplyStatusEffect(effectName, randomStacks, this);
                Debug.Log(effectName + " " + randomStacks + " [" + minStacks + "; " + maxStacks + "]");
            }
        }

        DealDamage(target, new Damage(damage, damageType, this));

		AudioManager.Instance?.PlaySound(attackSound);

	}

	public override void BoostStatsByStrength(int strength)
	{
		float amount = Mathf.Pow(1 + multiplierPerLeftStrength, strength);

		minStacks = (int)(minStacks * amount);
		maxStacks = (int)(maxStacks * amount);

		base.BoostStatsByStrength(strength);
	}

    public override int TakeDamage(Damage sentDamage)
    {
        bool canDodge = (UnityEngine.Random.value < successDodgeChance);
        if (canDodge)
        {
            // Some animation indication here?
            sentDamage.amount = (int)(sentDamage.amount * dodgeMultiplier);
        }
        return base.TakeDamage(sentDamage);
    }

    public override void ApplyStatusEffect(string effectName, int stack, Character sender)
    {
        if (stack == 0)
        {
            return;
        }
        bool canDodge = (UnityEngine.Random.value < successDodgeChance);
        if (canDodge)
        {
            // Some animation indication here?
            stack = (int)(stack * dodgeMultiplier);
        }
        base.ApplyStatusEffect(effectName, stack, sender);
    }

}
