using UnityEngine;
using Utils;

public class Inventory : MonoBehaviour
{
	public static Inventory Instance { get; private set; }

	[SerializeField] private ItemSlot[] itemSlots;
	public ItemSlot[] ItemSlots => itemSlots;

	private int pendingTileListeners;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		itemSlots = GetComponentsInChildren<ItemSlot>();
	}

	private void Start()
	{
		EnemyWaveManager.Instance.OnWaveSpawned += HandleWaveSpawned;
		EnemyWaveManager.Instance.OnEnemyTakeDamage += HandleEnemyTakeDamage;
		EnemyWaveManager.Instance.OnEnemyPreTakeDamage += HandleEnemyPreTakeDamage;
		EnemyWaveManager.Instance.OnEnemyTakeExcessiveDamage += HandleEnemyTakeExcessiveDamage;
		EnemyWaveManager.Instance.OnEnemyApplyStatusEffect += HandleEnemyApplyStatusEffect;
		EnemyWaveManager.Instance.OnEnemyPreApplyStatusEffect += HandleEnemyPreApplyStatusEffect;
		Player.Instance.OnPreApplyStatusEffect += HandlePlayerPreApplyStatusEffect;
		EnemyWaveManager.Instance.OnEnemyDefendDamage += HandleEnemyDefendDamage;
		TileEventBus.OnTileMatched += HandleTileMatched;
		Player.Instance.OnTakeDamage += HandlePlayerTakeDamage;
		Player.Instance.OnPreTakeDamage += HandlePlayerPreTakeDamage;
		Player.Instance.OnHeal += HandlePlayerHeal;
		Player.Instance.OnAddArmor += HandlePlayerAddArmor;
		Player.Instance.OnPreHeal += HandlePlayerPreHeal;
		Player.Instance.OnPreAddArmor += HandlePlayerPreAddArmor;
		Player.Instance.OnDefendDamage += HandlePlayerDefendDamage;
		BattleManager.EventBus.OnPlayerTurnStart += HandlePlayerTurnStart;
		BattleManager.EventBus.OnPlayerTurnEnd += HandlePlayerTurnEnd;
	}

	private void OnDisable()
	{
		EnemyWaveManager.Instance.OnWaveSpawned -= HandleWaveSpawned;
		EnemyWaveManager.Instance.OnEnemyTakeDamage -= HandleEnemyTakeDamage;
		EnemyWaveManager.Instance.OnEnemyTakeExcessiveDamage -= HandleEnemyTakeExcessiveDamage;
		EnemyWaveManager.Instance.OnEnemyPreTakeDamage -= HandleEnemyPreTakeDamage;
		EnemyWaveManager.Instance.OnEnemyApplyStatusEffect -= HandleEnemyApplyStatusEffect;
		EnemyWaveManager.Instance.OnEnemyPreApplyStatusEffect -= HandleEnemyPreApplyStatusEffect;
		EnemyWaveManager.Instance.OnEnemyDefendDamage -= HandleEnemyDefendDamage;
		Player.Instance.OnPreApplyStatusEffect -= HandlePlayerPreApplyStatusEffect;
		TileEventBus.OnTileMatched -= HandleTileMatched;
		Player.Instance.OnTakeDamage -= HandlePlayerTakeDamage;
		Player.Instance.OnPreTakeDamage -= HandlePlayerPreTakeDamage;
		Player.Instance.OnHeal -= HandlePlayerHeal;
		Player.Instance.OnAddArmor -= HandlePlayerAddArmor;
		Player.Instance.OnPreHeal -= HandlePlayerPreHeal;
		Player.Instance.OnPreAddArmor -= HandlePlayerPreAddArmor;
		Player.Instance.OnDefendDamage -= HandlePlayerDefendDamage;
		BattleManager.EventBus.OnPlayerTurnStart -= HandlePlayerTurnStart;
		BattleManager.EventBus.OnPlayerTurnEnd -= HandlePlayerTurnEnd;
	}

	public bool TryAddItem(Item item)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.CanAcceptItem(item))
			{
				slot.AssignItem(item);
				item.Equip(Player.Instance);
				return true;
			}
		}

		return false;
	}

	public void RemoveItem(Item item)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem == item)
			{
				slot.RemoveItem();
				item.Unequip();
				return;
			}
		}
	}

	public void RemoveItemfromSlot()
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem != null)
			{
				Item item = slot.EquippedItem;
				item.Unequip();
				slot.RemoveItem();
				Destroy(item.gameObject);
				return;
			}
		}
	}

	public void CancelAllDraggingItems()
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem != null)
			{
				slot.EquippedItem.CancelDragging();

				if (slot.EquippedItem.CurrentSlot == slot)
				{
					slot.AssignItem(slot.EquippedItem);
				}
			}
		}
	}

	public bool IsInventoryFull()
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem == null)
				return false;
		}
		return true;
	}

	public void NotifyTileItemFinished()
	{
		pendingTileListeners--;
		if (pendingTileListeners <= 0)
		{
			pendingTileListeners = 0;
			TileEventBus.TileMatchingProcessed();
		}
	}

	private void HandlePlayerHeal(int amount)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerHealItem listener)
			{
				listener.OnPlayerHeal(amount);
			}
		}
	}

	private void HandlePlayerAddArmor(int amount)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerAddArmorItem listener)
			{
				listener.OnPlayerAddArmor(amount);
			}
		}
	}

	private void HandlePlayerPreHeal(IntRef heal)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerPreHealItem listener)
			{
				listener.OnPlayerPreHeal(heal);
			}
		}
	}

	private void HandlePlayerPreAddArmor(IntRef amount)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerPreAddArmorItem listener)
			{
				listener.OnPlayerPreAddArmor(amount);
			}
		}
	}

	private void HandlePlayerTakeDamage(Damage damage)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerTakeDamageItem listener)
			{
				listener.OnPlayerTakeDamage(damage);
			}
		}
	}

	private void HandlePlayerPreTakeDamage(Damage damage)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerPreTakeDamageItem listener)
			{
				listener.OnPlayerPreTakeDamage(damage);
			}
		}
	}

	private void HandlePlayerDefendDamage(Damage damage)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerDefendDamageItem listener)
			{
				listener.OnPlayerDefendDamage(damage);
			}
		}
	}

	private void HandleEnemyDefendDamage(Enemy enemy, Damage damage)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnEnemyDefendDamageItem listener)
			{
				listener.OnEnemyDefendDamage(enemy, damage);
			}
		}
	}

	private void HandleWaveSpawned(Enemy[] wave)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnWaveSpawnItem listener)
			{
				listener.OnWaveSpawn(wave);
			}
		}
	}

	private void HandleEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnEnemyPreTakeDamageItem listener)
			{
				listener.OnEnemyPreTakeDamage(enemy, damage);
			}
		}
	}

	private void HandleEnemyTakeDamage(Enemy enemy, Damage damage)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnEnemyTakeDamageItem listener)
			{
				listener.OnEnemyTakeDamage(enemy, damage);
			}
		}
	}

	private void HandleEnemyTakeExcessiveDamage(Enemy enemy, Damage damage)
	{
		foreach (ItemSlot slot in ItemSlots)
		{
			if (slot.EquippedItem is IOnEnemyTakeExcessiveDamageItem listener)
			{
				listener.OnEnemyTakeExcessiveDamage(enemy, damage);
			}
		}
	}

	private void HandleEnemyApplyStatusEffect(Enemy enemy, string effectName, int stackCount)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnEnemyApplyStatusEffectItem listener)
			{
				listener.OnEnemyApplyStatusEffect(enemy, effectName, stackCount);
			}
		}
	}

	private void HandleEnemyPreApplyStatusEffect(Enemy enemy, string effectName, IntRef stackCount)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnEnemyPreApplyStatusEffectItem listener)
			{
				listener.OnEnemyPreApplyStatusEffect(enemy, effectName, stackCount);
			}
		}
	}

	private void HandlePlayerPreApplyStatusEffect(string effectName, IntRef stackCount)
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerPreApplyStatusEffectItem listener)
			{
				listener.OnPlayerPreApplyStatusEffect(effectName, stackCount);
			}
		}
	}

	private void HandleTileMatched(Tile tile, int count)
	{
		pendingTileListeners = 0;

		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnTileMatchItem)
				pendingTileListeners++;
		}

		if (pendingTileListeners == 0)
		{
			TileEventBus.TileMatchingProcessed();
			return;
		}

		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnTileMatchItem listener)
			{
				listener.OnTileMatched(tile, count);
			}
		}
	}

	private void HandlePlayerTurnEnd()
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerEndTurnItem listener)
			{
				listener.OnPlayerEndTurn();
			}
		}
	}

	private void HandlePlayerTurnStart()
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem is IOnPlayerStartTurnItem listener)
			{
				listener.OnPlayerStartTurn();
			}
		}
	}

	public void CloseItemUI()
	{
		foreach (ItemSlot slot in itemSlots)
		{
			if (slot.EquippedItem != null)
			{
				slot.EquippedItem.Open(false);
			}
		}
	}
}