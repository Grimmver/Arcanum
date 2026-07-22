using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class DamageTextAnimation : IFloatingTextAnimationStrategy
{
	private readonly float arcHeight;
	private readonly float duration;
	private readonly float sideOffset;
	private readonly float direction;

	public DamageTextAnimation(float direction, float arcHeight = 1f, float duration = 1f, float sideOffset = 0.5f)
	{
		this.arcHeight = arcHeight;
		this.duration = duration;
		this.sideOffset = sideOffset;
		this.direction = Mathf.Sign(direction);
	}

	public void Animate(Transform t, TMP_Text text, Action onComplete)
	{
		Vector3 startPos = t.position;

		Vector3 endPos = startPos + new Vector3(direction * sideOffset, 0f, 0f);
		Vector3 controlPoint = startPos + new Vector3(direction * sideOffset * 0.5f, arcHeight, 0f);

		float tVal = 0f;
		Vector3 p0 = startPos;
		Vector3 p1 = controlPoint;
		Vector3 p2 = endPos;

		DOTween.To(() => tVal, x =>
		{
			tVal = x;
			t.position = Mathf.Pow(1 - tVal, 2) * p0 + 2 * (1 - tVal) * tVal * p1 + Mathf.Pow(tVal, 2) * p2;
		}, 1f, duration)
		.SetEase(Ease.Linear)
		.OnComplete(() => onComplete?.Invoke());
	}
}
