using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ExplodingStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public int requiredStacks; // Amount of stacks required to cast new effect.
    [SerializeField] public float damageInitial; // Amount of damage applied for getting requiredStacks.
    [SerializeField] public float damageConsequent; // Amount of extra damage applied per each extra requiredStacks.
    [SerializeField] public DamageType damageType;
    // Add something about AOE...

    public override void ProcessStatusEffect(Action onComplete)
    {

        if (GetCurrentStack() < requiredStacks)
        {
            _baseEffect.ProcessStatusEffect(() =>
                {
                    onComplete?.Invoke();
                }
            );
            return;
        }

        Character originalStackOwner = GetOldestStack();

        int streaks = GetCurrentStack() / requiredStacks, totalStacksUsed = streaks * requiredStacks;

        // DecrementStack(totalStacksUsed);

        GetOwner().TakeDamage(new Damage((int)(damageInitial + (streaks - 1) * damageConsequent), damageType, originalStackOwner));
        // Add something about AOE...

        _baseEffect.ProcessStatusEffect(() =>
            {
                onComplete?.Invoke();
            }
        );
    }

}
