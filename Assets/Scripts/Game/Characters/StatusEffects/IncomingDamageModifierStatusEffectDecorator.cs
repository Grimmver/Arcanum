using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class IncomingDamageModifierStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public float incomingDamageModifierPerStack;
    [SerializeField] public float incomingDamageModifierBase;

    public override float GetIncomingDamageModifier()
    {
        return (float)Math.Pow(incomingDamageModifierPerStack, GetCurrentStack()) - 1f + _baseEffect.GetIncomingDamageModifier() + incomingDamageModifierBase;
    }

}
