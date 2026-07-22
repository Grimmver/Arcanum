using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class BlockTextAnimation : IFloatingTextAnimationStrategy
{
	private readonly float shakeDuration;
	private readonly float shakeStrength;

	public BlockTextAnimation(float shakeDuration = 1f, float shakeStrength = 0.3f)
	{
		this.shakeDuration = shakeDuration;
		this.shakeStrength = shakeStrength;
	}

	public void Animate(Transform t, TMP_Text text, Action onComplete)
	{
		t.DOShakePosition(shakeDuration, new Vector3(shakeStrength, 0f, 0f), 10)
			.OnComplete(() => onComplete?.Invoke());
	}
}
