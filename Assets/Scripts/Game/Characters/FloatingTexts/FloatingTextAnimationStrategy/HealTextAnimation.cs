using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class HealTextAnimation : IFloatingTextAnimationStrategy
{
	private readonly float floatDistance;
	private readonly float floatDuration;

	public HealTextAnimation(float floatDistance = 1f, float floatDuration = 0.75f)
	{
		this.floatDistance = floatDistance;
		this.floatDuration = floatDuration;
	}

	public void Animate(Transform t, TMP_Text text, Action onComplete)
	{
		var canvasGroup = t.GetComponent<CanvasGroup>() ?? t.gameObject.AddComponent<CanvasGroup>();
		canvasGroup.alpha = 1;

		DOTween.Sequence()
			.Join(t.DOMove(t.position + Vector3.up * floatDistance, floatDuration))
			.Join(canvasGroup.DOFade(0f, floatDuration))
			.OnComplete(() => onComplete?.Invoke());
	}
}
