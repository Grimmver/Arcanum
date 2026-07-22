using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DamageDecrementStatusEffectDecorator : StatusEffectDecorator
{

    [SerializeField] public int decrementPerDamage = 1;

    public override void ParseDamage(Damage damage)
    {
        DecrementStack(decrementPerDamage);
        base.ParseDamage(damage);
    }

}
