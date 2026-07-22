using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
	public static Shop Instance { get; private set; }

	[Header("General")]
	public GameObject holderObject;
	[SerializeField] private Transform inventoryHolder;
	[SerializeField] private Vector3 inventoryOriginalPosition;
	[SerializeField] private UI.ShopCanvas shopCanvas;
	public int shopEncounterCount;

	[Header("Reroll")]
	public int rerollCost;
	public int rerollCount = 0;
	[SerializeField] private int rerollBaseCost = 20;
	[SerializeField] private int rerollCostIncreasePerUse = 10;

	[Header("Dictionary")]
	[SerializeField] private List<ShopItemSlot> shopItemSlots;
	[SerializeField] public List<Item> shopItemPrefabs;
	private HashSet<Item> soldItems = new HashSet<Item>();

	[Header("Healing")]
	[SerializeField] private int baseHealingCost = 200;
	[SerializeField] private int nextShopHealingCostIncrease = 100;
	private bool isHealAvailable;
	private int currentHealingCost;

	[Header("Item Remove")]
	[SerializeField] private int baseRemoveCost = 300;
	[SerializeField] private int nextShopRemoveCostIncrease = 100;
	[SerializeField] private int lategameshopstart = 7;
	private bool isRemoveAvailable;
	private int currentRemoveCost;

	private const string CRYSTAL_PREF = "Crystal";

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		rerollCost = rerollBaseCost;
	}

	private void Start()
	{
		inventoryOriginalPosition = Inventory.Instance.transform.localPosition;
		TryUpdateShop();
		Open(false);
	}

	public void Reroll()
	{
		if (rerollCount <= 0)
		{
			int crystal = PlayerPrefsEventBus.GetInt(CRYSTAL_PREF, 100);
			if (crystal >= rerollCost)
			{
				if (TryUpdateShop())
				{
					PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, crystal - rerollCost);
					rerollCost += rerollCostIncreasePerUse;
				}
			}
		}
		else
		{
			if (TryUpdateShop())
			{
				rerollCount--;
			}
		}
	}

	private bool TryUpdateShop()
	{
		if (shopItemSlots.Count == 0 || shopItemPrefabs.Count == 0) return false;

		var equippedItems = Inventory.Instance.ItemSlots
			.Select(s => s.EquippedItem)
			.Where(i => i != null)
			.ToList();

		List<Item> validCandidates = new();

		foreach (Item itemPrefab in shopItemPrefabs)
		{
			if (itemPrefab == null || soldItems.Contains(itemPrefab)) continue;

			bool isInvalid = false;

			foreach (Item equipped in equippedItems)
			{
				// Reject if same item by name
				if (equipped.ItemName == itemPrefab.ItemName)
				{
					isInvalid = true;
					break;
				}

				// Reject if incompatible
				if (itemPrefab.IncompatibleItems.Contains(equipped) || equipped.IncompatibleItems.Contains(itemPrefab))
				{
					isInvalid = true;
					break;
				}
			}

			if (!isInvalid)
				validCandidates.Add(itemPrefab);
		}

		if (validCandidates.Count < shopItemSlots.Count) return false;

		Dictionary<ItemRarity, List<Item>> validCandidatesByRarity = new Dictionary<ItemRarity, List<Item>>();

		foreach (ItemRarity rarity in Enum.GetValues(typeof(ItemRarity)))
		{
			validCandidatesByRarity[rarity] = new List<Item>();
		}

		foreach (Item item in validCandidates)
		{
			validCandidatesByRarity[item.Rarity].Add(item);
		}

		// foreach (ItemRarity rarity in Enum.GetValues(typeof(ItemRarity)))
		// {
		// 	Debug.Log(rarity + " -- " + validCandidatesByRarity[rarity].Count);
		// }

		List<Item> selectedItems = new();

		// Fill selected items...
		int attemptNumber = 0;
		while (selectedItems.Count < shopItemSlots.Count && attemptNumber < 1000)
		{
			attemptNumber++;
			ItemRarity rarity = GetRandomRarity();
			int availableItemCount = validCandidatesByRarity[rarity].Count;
			if (availableItemCount == 0)
			{
				continue;
			}
			int itemIndex = UnityEngine.Random.Range(0, availableItemCount);
			selectedItems.Add(validCandidatesByRarity[rarity][itemIndex]);
			validCandidatesByRarity[rarity].RemoveAt(itemIndex);
		}

		if (selectedItems.Count < shopItemSlots.Count) return false;

		int index = 0;
		foreach (ShopItemSlot slot in shopItemSlots)
		{
			if (slot.IsSold) continue;
			if (index >= selectedItems.Count) break;

			slot.SetItem(selectedItems[index]);
			index++;
		}

		return true;
	}

	public static ItemRarity GetRandomRarity()
	{

		int totalWeight = 0;
		foreach (var weight in RarityChances.Weights.Values)
		{
			totalWeight += weight;
		}

		int randomValue = UnityEngine.Random.Range(1, totalWeight + 1);

		int cumulativeWeight = 0;

		foreach (var rarity in RarityChances.Weights)
		{
			cumulativeWeight += rarity.Value;
			if (randomValue <= cumulativeWeight)
			{
				return rarity.Key;
			}
		}

		return ItemRarity.Common;
	}

	public void HandleItemSold(Item soldItem)
	{
		if (soldItem == null || soldItems.Contains(soldItem)) return;

		soldItems.Add(soldItem);

		foreach (ShopItemSlot slot in shopItemSlots)
		{
			if (slot.ItemPrefab == soldItem)
			{
				slot.Sold();
			}
		}
	}
    public void UpdateItemSoldList(Item soldItem)
    {
        if (soldItem == null || soldItems.Contains(soldItem)) return;
		soldItems.Add(soldItem);

    }

    public bool AreAllSlotsSold() => shopItemSlots.All(slot => slot.IsSold);

	private void ResetAllSlots()
	{
		foreach (ShopItemSlot slot in shopItemSlots)
		{
			slot.ResetSlot();
		}
	}

	public void Open(bool action)
	{

		if (Inventory.Instance == null) return;

		if (action)
		{

			shopEncounterCount = EnemyWaveManager.Instance.TurnCounter / 5;

			ResetAllSlots();
			TryUpdateShop();

			holderObject.SetActive(true);
			Inventory.Instance?.CancelAllDraggingItems();
			//Time.timeScale = 0;
			Inventory.Instance.transform.position = inventoryHolder.position;
			rerollCost = rerollBaseCost;

			isHealAvailable = true;
			isRemoveAvailable = true;
			currentHealingCost = baseHealingCost + shopEncounterCount * nextShopHealingCostIncrease;
			currentRemoveCost = baseRemoveCost + shopEncounterCount * shopEncounterCount * nextShopRemoveCostIncrease / lategameshopstart;
			shopCanvas.UpdateHealingUI(currentHealingCost.ToString(), Color.white);
			shopCanvas.UpdateRemoveUI(currentRemoveCost.ToString(), Color.white);

			if (ShopSell.Instance != null)
			{
				ShopSell.Instance.currentShopSold = 0;
				ShopSell.Instance.sellAmount = baseRemoveCost + shopEncounterCount * shopEncounterCount * nextShopRemoveCostIncrease / lategameshopstart;
				ShopSell.Instance.UpdateUI();
			}

			// shopEncounterCount++;

		}
		else
		{
			holderObject.SetActive(false);
			Inventory.Instance?.CancelAllDraggingItems();
			//Time.timeScale = 1f;
			Inventory.Instance.transform.localPosition = inventoryOriginalPosition;
		}
	}

	public void OnHealButtonClick()
	{

		if (isHealAvailable == false)
		{
			return;
		}

		int crystal = PlayerPrefsEventBus.GetInt(CRYSTAL_PREF, 100);
		if (crystal >= currentHealingCost)
		{
			isHealAvailable = false;
			PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, crystal - currentHealingCost);
			Player.Instance.Health += (int)(Player.Instance.MaxHealth * 0.40f);
			shopCanvas.UpdateHealingUI("SOLD", Color.gray);
		}
	}

	public void OnRemoveButtonClick()
	{

		if (isRemoveAvailable == false)
		{
			return;
		}

		int crystal = PlayerPrefsEventBus.GetInt(CRYSTAL_PREF, 100);
		if (crystal >= currentRemoveCost)
		{
			isRemoveAvailable = false;
			PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, crystal - currentRemoveCost);
			Inventory.Instance?.RemoveItemfromSlot();
			shopCanvas.UpdateRemoveUI("SOLD", Color.gray);
		}
	}
}
