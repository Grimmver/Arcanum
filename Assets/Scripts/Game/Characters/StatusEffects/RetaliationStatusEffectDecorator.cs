using System;
using UnityEngine;
using UnityEngine.UI;

public class RetaliationStatusEffectDecorator : StatusEffectDecorator
{

    public DamageType damageType;
    public int damageAmount;

    public override void ParseDamage(Damage damage)
    {
        if (damage.sender != null && !damage.isBonusDamage && damage.statusEffectName == "")
        {
            GetOwner().DealDamage(damage.sender, new Damage(damageAmount, damageType, GetOwner()));
            DecrementStack();
        }
        base.ParseDamage(damage);
    }
    
}
