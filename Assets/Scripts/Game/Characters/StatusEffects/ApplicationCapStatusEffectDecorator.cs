using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ApplicationCapStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public int stackCap = 1;

    public override void Init(Character owner, int stack, Character sender)
    {
        int carryoverStack = Math.Min(stackCap, stack);
        base.Init(owner, carryoverStack, sender);
    }

    public override void ApplyAdditionalStack(int amount, Character sender)
    {
        int carryoverStack = Math.Max(Math.Min(stackCap - GetCurrentStack(), amount), 0);
        base.ApplyAdditionalStack(carryoverStack, sender);
    }

}
