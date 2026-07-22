using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class OutgoingDamageModifierStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public float outgoingDamageModifierPerStack;

    public override float GetOutgoingDamageModifier()
    {
        return (float)Math.Pow(outgoingDamageModifierPerStack, GetCurrentStack()) - 1f + _baseEffect.GetOutgoingDamageModifier();
    }

}
