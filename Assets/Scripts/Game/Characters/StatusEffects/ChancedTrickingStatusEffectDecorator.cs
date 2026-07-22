using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ChancedTrickingStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public int requiredStacks;
    [Range(0f, 1f)] [SerializeField] public float chance;

    public override bool IsTricking()
    {

        if (GetCurrentStack() < requiredStacks || UnityEngine.Random.value > chance)
        {
            return _baseEffect.IsTricking();
        }

        DecrementStack(requiredStacks);

        return true;
    }

}
