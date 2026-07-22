using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ProcessMultiplyStatusEffectDecorator : StatusEffectDecorator
{

    [Header("Balance")]
    [SerializeField] public int copyCountToAdd = 1;
    [Range(0f, 1f)] [SerializeField] public float chance;

    public override void ProcessStatusEffect(Action onComplete)
    {

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll <= chance)
        {
            List <Character> stackList = GetStackList();
            foreach (Character character in stackList)
            {
                ApplyAdditionalStack(copyCountToAdd, character);
            }
        }

        _baseEffect.ProcessStatusEffect(() => 
            {
                onComplete?.Invoke();
            }
        );
    }

}
