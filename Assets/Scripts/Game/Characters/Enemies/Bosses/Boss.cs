using UnityEngine;

public class Boss : Enemy
{
	[SerializeField] protected BossCurse curse;

	protected override void Awake()
	{
		base.Awake();


	}

	protected virtual void ApplyCurse()
	{

	}
}
