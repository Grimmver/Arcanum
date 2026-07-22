using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ChancedStunningStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public int requiredStacks;
    [Range(0f, 1f)] [SerializeField] public float chance;

    public override bool IsStunning()
    {

        if (GetCurrentStack() < requiredStacks || UnityEngine.Random.value > chance)
        {
            return _baseEffect.IsStunning();
        }

        DecrementStack(requiredStacks);

        return true;
    }

}
