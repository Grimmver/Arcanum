using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ProcessDecrementStatusEffectDecorator : StatusEffectDecorator
{

    [SerializeField] public int decrementPerProcess = 1;

    public override void ProcessStatusEffect(Action onComplete)
    {

        _baseEffect.ProcessStatusEffect(() => 
            {
                DecrementStack(decrementPerProcess);
                onComplete?.Invoke();
            }
        );
    }

}
