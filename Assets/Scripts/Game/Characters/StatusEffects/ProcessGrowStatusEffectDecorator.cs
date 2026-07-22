using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ProcessGrowStatusEffectDecorator : StatusEffectDecorator
{

    public float animationUpLength = 0.5f;
    public float animationDownLength = 0.5f;

    public override void ProcessStatusEffect(Action onComplete)
    {

        Image icon = GetIcon();

        if (icon != null)
        {
            icon.rectTransform.DOScale(1.5f, animationUpLength)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    if (this == null || icon == null)
                    {
                        onComplete?.Invoke(); return;
                    }

                    _baseEffect.ProcessStatusEffect(() =>
                        {

                            if (this == null || icon == null)
                            {
                                onComplete?.Invoke(); return;
                            }

                            icon.rectTransform.DOScale(1f, animationDownLength)
                            .SetEase(Ease.InBack)
                            .SetLink(gameObject)
                            .OnComplete(() =>
                            {
                                onComplete?.Invoke();
                            }
                            );

                        }
                    );
                    
                });
        }
        else
        {
            Debug.LogWarning("Icon not assigned for status effect!");
            onComplete?.Invoke();
        }
    }
}
