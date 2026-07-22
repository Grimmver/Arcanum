using DG.Tweening;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class BlinkingObject : MonoBehaviour
	{
		[SerializeField] private float blinkDuration = 0.5f;
		[SerializeField] private bool playOnStart = true;
		[SerializeField][Range(0f, 1f)] private float minAlpha = 0f;
		[SerializeField][Range(0f, 1f)] private float maxAlpha = 1f;

		private CanvasGroup canvasGroup;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start()
		{
			if (playOnStart) StartBlinking();
		}

		private void OnDisable() => DOTween.Kill(gameObject);

		public void StartBlinking()
		{
			canvasGroup.alpha = maxAlpha;

			canvasGroup
				.DOFade(minAlpha, blinkDuration)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine)
				.SetLink(gameObject);
		}
	}
}
