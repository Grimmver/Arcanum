using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ChancedOutgoingDamageModifierStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public float outgoingDamageModifier;
    [Range(0f, 1f)] [SerializeField] public float chance;

    public override float GetOutgoingDamageModifier()
    {
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll > chance)
        {
            return _baseEffect.GetOutgoingDamageModifier();
        }
        return outgoingDamageModifier + _baseEffect.GetOutgoingDamageModifier();
    }

}
