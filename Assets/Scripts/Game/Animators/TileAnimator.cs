using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public class TileAnimator : MonoBehaviour
{
	[SerializeField] private AnimationCurve dropCurve;
	[SerializeField] private AnimationCurve swapCurveMove;
	[SerializeField] private AnimationCurve swapCurveRotate;

	public void AnimateElementDrop(Vector3 targetPos)
	{
		transform.DOMove(targetPos, 0.5f).SetEase(dropCurve);
	}

	public void SwapAnimation(Vector3 targetPos, float swapDuration)
	{
		if (Maths.ApproximatelyEqual(targetPos.x, transform.position.x))
		{
			transform.DOMoveY(targetPos.y, swapDuration).SetEase(swapCurveMove);
			transform.DOMoveX(transform.position.x + transform.position.y - targetPos.y, swapDuration).SetEase(swapCurveRotate);
		}
		else
		{
			transform.DOMoveX(targetPos.x, swapDuration).SetEase(swapCurveMove);
			transform.DOMoveY(transform.position.y + transform.position.x - targetPos.x, swapDuration).SetEase(swapCurveRotate);
		}
	}
}
