using DG.Tweening;
using System;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	public class FallingObject : MonoBehaviour
	{
		[SerializeField] private float fallDistance = 200f;
		[SerializeField] private float fallDuration = 0.5f;
		[SerializeField] private float fallDelayOnStart = 0.2f;
		[SerializeField] private Ease fallEase = Ease.OutBounce;

		private RectTransform rectTransform;
		private Vector3 originalPosition;

		public event Action OnFallComplete;

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			originalPosition = rectTransform.anchoredPosition;
		}

		private void Start()
		{
			rectTransform.anchoredPosition += new Vector2(0, fallDistance);

			rectTransform
				.DOAnchorPos(originalPosition, fallDuration)
				.SetDelay(fallDelayOnStart)
				.SetEase(fallEase)
				.SetLink(gameObject)
				.OnComplete(() => OnFallComplete?.Invoke());
		}
	}
}
