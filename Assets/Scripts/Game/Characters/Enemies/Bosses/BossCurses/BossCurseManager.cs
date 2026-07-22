using System.Collections.Generic;
using UnityEngine;

public class BossCurseManager : MonoBehaviour
{
	public static BossCurseManager Instance { get; private set; }

	[SerializeField] private List<BossCurseHolder> curseHolders;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}
}
