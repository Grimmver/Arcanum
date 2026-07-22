using UnityEngine;

public class BossCurseHolder : MonoBehaviour
{
	[SerializeField] private BossCurse curse;
	public BossCurse Curse => curse;

	public void Activate()
	{
		if (curse == null) return;

		curse.Activate();
	}
}
