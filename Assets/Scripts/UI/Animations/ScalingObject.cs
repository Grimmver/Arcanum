using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class ScalingObject : MonoBehaviour
	{
		[SerializeField] private bool playOnStart = false;
		[SerializeField] private float scaleUpTo = 1.1f;
		[SerializeField] private float duration = 0.5f;
		[SerializeField] private Ease ease = Ease.InOutSine;

		private void Start()
		{
			if (playOnStart) StartScaling();
		}

		private void OnDisable()
		{
			DOTween.Kill(transform);
		}

		public void StartScaling()
		{
			transform.localScale = Vector3.one;

			transform
				.DOScale(scaleUpTo, duration)
				.SetEase(ease)
				.SetLoops(-1, LoopType.Yoyo);
		}
	}
}
