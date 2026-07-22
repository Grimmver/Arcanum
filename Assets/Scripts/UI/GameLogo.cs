using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(FallingObject), typeof(ScalingObject))]
	public class GameLogoController : MonoBehaviour
	{
		private FallingObject fallingObject;
		private ScalingObject scalingObject;

		private void Awake()
		{
			fallingObject = GetComponent<FallingObject>();
			scalingObject = GetComponent<ScalingObject>();
		}

		private void OnEnable()
		{
			fallingObject.OnFallComplete += scalingObject.StartScaling;
		}

		private void OnDisable()
		{
			fallingObject.OnFallComplete -= scalingObject.StartScaling;
		}
	}
}
