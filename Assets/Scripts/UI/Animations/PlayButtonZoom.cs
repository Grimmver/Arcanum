using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class PlayButtonZoom : MonoBehaviour
	{
		[SerializeField] private RectTransform mainCanvas;
		[SerializeField] private RectTransform targetPosition;
		[SerializeField] private float zoomScale = 2f;
		[SerializeField] private float duration = 0.8f;
		[SerializeField] private string onClickSound = "Play button";

		private void Start()
		{
			if (mainCanvas == null)
			{
				mainCanvas = transform.parent.GetComponent<RectTransform>();
			}

			if (targetPosition == null)
			{
				targetPosition = transform.parent.Find("DungeonEnterRectTransform").GetComponent<RectTransform>();
			}
		}

		public void OnPlayButtonClick()
		{
			AudioManager.Instance?.PlaySound(onClickSound);
			ZoomIntoDoor();
		}

		public void ZoomIntoDoor()
		{
			Vector2 localTargetPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				mainCanvas, targetPosition.position, null, out localTargetPos);

			Vector2 offset = localTargetPos * (zoomScale - 1f);

			Sequence zoomSequence = DOTween.Sequence();
			zoomSequence.Join(mainCanvas.DOScale(zoomScale, duration).SetEase(Ease.InOutSine));
			zoomSequence.Join(mainCanvas.DOAnchorPos(-offset, duration).SetEase(Ease.InOutSine));
			zoomSequence.OnComplete(() =>
			{
				MainCanvas.Instance?.OnPlayButtonClick();
			});
		}
	}
}
