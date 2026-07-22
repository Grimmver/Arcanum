using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingText : MonoBehaviour
{
	[SerializeField] private TMP_Text text;

	private IFloatingTextAnimationStrategy animationStrategy;

	public void Initialize(string value, Color color, IFloatingTextAnimationStrategy strategy)
	{
		text.text = value;
		text.color = color;
		animationStrategy = strategy;

		AnimateAndDestroy();
	}

	private void AnimateAndDestroy()
	{
		animationStrategy.Animate(transform, text, () =>
		{
			Destroy(gameObject);
		});
	}
}
