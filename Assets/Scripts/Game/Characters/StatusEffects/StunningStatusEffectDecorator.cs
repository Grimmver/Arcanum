using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class StunningStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public int requiredStacks;

    public override bool IsStunning()
    {

        if (GetCurrentStack() < requiredStacks)
        {
            return false;
        }

        DecrementStack(requiredStacks);

        return true;
    }

}
