using System.Collections;
using UnityEngine;

public class VampireBat : Enemy
{

	[SerializeField] private float healCoefficient;

	public override void Attack(Character target)
	{
		int damage = Random.Range(minAttackDamage, maxAttackDamage + 1);

		int damageDealt = DealDamage(target, new Damage(damage, damageType, this));

		int healAmount = (int)(damageDealt * healCoefficient);

		if (healAmount > 0)
		{
			Heal(healAmount);
		}

		AudioManager.Instance?.PlaySound(attackSound);
	}

}
