using System.Collections;
using UnityEngine;

public class Spider : Enemy
{
	[SerializeField] private int minStacks = 0;
	[SerializeField] private int maxStacks = 1;

	public override void Attack(Character target)
	{
		int damage = Random.Range(minAttackDamage, maxAttackDamage + 1);

		int damageDealt = DealDamage(target, new Damage(damage, damageType, this));

        // if (damageDealt > 0){
            int randomStacks = Random.Range(minStacks, maxStacks + 1);
            target.ApplyStatusEffect("EnemyPoison", randomStacks, this);
        // }

		AudioManager.Instance?.PlaySound(attackSound);
	}

	public override void BoostStatsByStrength(int strength)
	{
		float amount = Mathf.Pow(1 + multiplierPerLeftStrength, strength);

		minStacks = (int)(minStacks * amount);
		maxStacks = (int)(maxStacks * amount);

		base.BoostStatsByStrength(strength);
	}

}
