using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class BattleManager : MonoBehaviour
{
	static public BattleManager instance { get; private set; }

	[SerializeField] public Player player;
	[SerializeField] private List<Enemy> CurrentEnemies;
	[SerializeField] private MatchManager mbg;

	public bool IsProcessingMatch = false;

	public static class EventBus
	{
		public static event Action OnTurnChange;
		public static event Action OnPlayerTurnStart;
		public static event Action OnPlayerSwapTurn;
		public static event Action OnPlayerTurnEnd;
		public static event Action OnEnemyTurnStart;
		public static event Action OnEnemyTurnEnd;
		public static event Action OnPlayerSwapped;
		public static event Action OnPlayerSwapTurnFinished;

		public static void TurnChange() => OnTurnChange?.Invoke();
		public static void PlayerTurnStart()
		{
			OnPlayerTurnStart?.Invoke();
		}
		public static void PlayerSwapTurn() => OnPlayerSwapTurn?.Invoke();
		public static void EnemyTurnStart() => OnEnemyTurnStart?.Invoke();
		public static void EnemyTurnEnd()
		{
			OnEnemyTurnEnd?.Invoke();
			BattleManager.instance.SetTurn(BattleManager.TurnPhase.PlayerTurn);
		}
		public static void PlayerTurnEnd() => OnPlayerTurnEnd?.Invoke();
		public static void PlayerSwapped() => OnPlayerSwapped?.Invoke();
		public static void PlayerSwapTurnFinished() => OnPlayerSwapTurnFinished?.Invoke();
	}

	public void SetCurrentEnemies(IEnumerable<Enemy> enemies)
	{
		CurrentEnemies = enemies.Where(e => e != null && !e.IsDying()).ToList();
	}

	public enum TurnPhase
	{
		PlayerTurn,
		PlayerSwaped,
		EnemyTurn,
	}

	public TurnPhase turnPhase = TurnPhase.PlayerTurn;

	public void ProcessTurn()
	{
		switch (turnPhase)
		{
			case TurnPhase.PlayerTurn:
				EventBus.PlayerTurnStart();
				break;
			case TurnPhase.PlayerSwaped:
				EventBus.PlayerSwapTurn();
				break;
			case TurnPhase.EnemyTurn:
				EventBus.EnemyTurnStart();
				break;
		}
	}

	public void SetTurn(TurnPhase turn)
	{
		turnPhase = turn;
		EventBus.TurnChange();
	}

	private void HandlePlayerTurn()
	{
		player.TakeTurn();
	}

	private void HandlePlayerSwapTurn()
	{
		mbg.SetCanInteract(false);
	}

	private void HandleEnemyTurn()
	{
		// mbg.SetCanInteract(false); // Not needed?
	}

	public void playerSwapped()
	{
		SetTurn(TurnPhase.PlayerSwaped);
	}

	public void playerSwapTurnFinished()
	{
		EventBus.PlayerTurnEnd();
		playerEffectProcessingFinished();
	}

	public void playerEffectProcessingFinished()
	{
		SetTurn(TurnPhase.EnemyTurn);
	}

	public void HandlePlayerAttack(Damage damage)
	{
		if (turnPhase != TurnPhase.PlayerTurn && turnPhase != TurnPhase.PlayerSwaped) return;

		// ?
		// IsProcessingMatch = true;

		if (damage != null)
		{
			Enemy target = GetEnemy();

			if (target != null)
			{
				target.TakeDamage(damage);
			}


			//
			//
			//
		}
	}

	public void HandlePlayerStatusEffectCast(string effectName, int stack, Character sender)
	{
		if (turnPhase != TurnPhase.PlayerTurn && turnPhase != TurnPhase.PlayerSwaped) return;

		// IDK.
		// IsProcessingMatch = true;

		if (effectName != null)
		{
			Enemy target = GetEnemy();

			if (target != null)
			{
				target.ApplyStatusEffect(effectName, stack, sender);
			}
		}
	}

	public Enemy GetEnemy()
	{
		return CurrentEnemies.FirstOrDefault(e => !e.IsDying() && !e.IsDead());
	}

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void StartBattle()
	{
		SetTurn(TurnPhase.PlayerTurn);
	}

	private void OnEnable()
	{
		EventBus.OnPlayerTurnStart += HandlePlayerTurn;
		EventBus.OnPlayerSwapTurn += HandlePlayerSwapTurn;
		EventBus.OnEnemyTurnStart += HandleEnemyTurn;
		EventBus.OnTurnChange += ProcessTurn;
		EventBus.OnPlayerSwapTurnFinished += playerSwapTurnFinished;
		EventBus.OnPlayerSwapped += playerSwapped;

		TileEventBus.OnTileMatchingProcessed += FinishProcessingMatch;
	}

	private void OnDisable()
	{
		EventBus.OnPlayerTurnStart -= HandlePlayerTurn;
		EventBus.OnPlayerSwapTurn -= HandlePlayerSwapTurn;
		EventBus.OnEnemyTurnStart -= HandleEnemyTurn;
		EventBus.OnTurnChange -= ProcessTurn;
		EventBus.OnPlayerSwapTurnFinished -= playerSwapTurnFinished;
		EventBus.OnPlayerSwapped -= playerSwapped;

		TileEventBus.OnTileMatchingProcessed -= FinishProcessingMatch;
	}

	void Start()
	{
		StartBattle();
	}

	public void FinishProcessingMatch()
	{
		IsProcessingMatch = false;
		if (turnPhase == TurnPhase.EnemyTurn)
		{
			EventBus.EnemyTurnEnd();
		}
	}
}
