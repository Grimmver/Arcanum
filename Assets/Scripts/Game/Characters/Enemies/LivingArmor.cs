using System.Collections;
using UnityEngine;

public class LivingArmor : Enemy
{

    [SerializeField] private int armorPerAttack;

	public override void Attack(Character target)
	{
        AddArmor(armorPerAttack);
        base.Attack(target);
	}

    public override void BoostStatsByStrength(int strength)
	{
		float amount = Mathf.Pow(1 + multiplierPerLeftStrength, strength);

		armorPerAttack = (int)(armorPerAttack * amount);

		base.BoostStatsByStrength(strength);
	}

}
