using TMPro;
using UnityEngine;

public interface IFloatingTextAnimationStrategy
{
	void Animate(Transform textTransform, TMP_Text textComponent, System.Action onComplete);
}
