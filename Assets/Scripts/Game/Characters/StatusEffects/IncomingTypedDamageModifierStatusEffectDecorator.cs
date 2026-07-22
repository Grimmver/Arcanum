using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class IncomingTypedDamageModifierStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public float incomingDamageModifierPerStack;
    [SerializeField] public float incomingDamageModifierBase;
    [SerializeField] public DamageType damageType;

    public override float GetIncomingTypedDamageModifier(DamageType incomingDamageType)
    {
        if (incomingDamageType != damageType)
        {
            return _baseEffect.GetIncomingTypedDamageModifier(incomingDamageType);
        }
        return (float)Math.Pow(incomingDamageModifierPerStack, GetCurrentStack()) - 1f + _baseEffect.GetIncomingTypedDamageModifier(incomingDamageType) + incomingDamageModifierBase;
    }

}
