using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
	public static Player Instance { get; private set; }

	[Header("Player Settings")]
	[SerializeField] private MatchManager mbg;
	[SerializeField] public Animator animator;
    [SerializeField] public SpriteRenderer Playersprite;
    public Image ArmorIcon;

	protected override void Awake()
	{
		base.Awake();
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
		}
	}

	public override void Die()
	{
		AudioManager.Instance?.PlaySound("Player death");
		LoseCanvas.Instance.Open(true);
		Time.timeScale = 0;
	}

	public override void TakeTurn()
	{
		if (BattleManager.instance.GetEnemy() == null || BattleManager.instance.GetEnemy().IsDying())
		{
			ClearStatusEffects();
			mbg.SetCanInteract(true);
		}
		ProcessStatusEffects(() => 
		{
			if (statusEffectManager.IsStunned())
			{
				StartCoroutine(SkipTurn());
			}
			else
			{
				mbg.SetCanInteract(true);
			}
		}
		);
	}

	public override IEnumerator SkipTurn()
	{
		yield return base.SkipTurn();
		BattleManager.EventBus.PlayerSwapTurnFinished();
	}
}
