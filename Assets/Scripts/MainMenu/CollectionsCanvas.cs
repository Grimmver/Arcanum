using System.Collections.Generic;
using TMPro;
using UI;
using UI.Collections;
using UnityEngine;

public enum CollectionType
{
	Item,
	Enemy,
	Tile
}

public class CollectionsCanvas : UICanvas<CollectionsCanvas>
{
	[Header("Collections")]
	[SerializeField] private TMP_Text currentCollectionsText;
	[SerializeField] private Transform collectionsContainer;
	[SerializeField] private GameObject collectionSlotPrefab;

	[SerializeField] private List<GameObject> itemCollectionsPrefabs;
	[SerializeField] private List<GameObject> enemyCollectionsPrefabs;
	[SerializeField] private List<GameObject> tileCollectionsPrefabs;

	private CollectionType currentCollectionType = CollectionType.Item;

	private Dictionary<CollectionType, List<GameObject>> collectionMap;

	protected override void Awake()
	{
		collectionMap = new()
		{
			{ CollectionType.Item, itemCollectionsPrefabs },
			{ CollectionType.Enemy, enemyCollectionsPrefabs },
			{ CollectionType.Tile, tileCollectionsPrefabs },
		};

		base.Awake();
	}

	public override void Open(bool action)
	{
		base.Open(action);
		if (action)
		{
			SpawnCollections(currentCollectionType);
		}
	}

	private void ClearSlots()
	{
		foreach (Transform child in collectionsContainer)
		{
			Destroy(child.gameObject);
		}
	}

	private void SpawnCollections(CollectionType type)
	{
		ClearSlots();

		currentCollectionsText.text = LocalizationUtility.GetLocalizedCollectionType(type);

		if (!collectionMap.TryGetValue(type, out var prefabs)) return;

		List<GameObject> sortedPrefabs = SortCollections(type, prefabs);

		foreach (GameObject prefab in sortedPrefabs)
		{
			if (prefab == null) continue;

			GameObject slotGO = Instantiate(collectionSlotPrefab, collectionsContainer);

			if (slotGO.TryGetComponent(out CollectionsSlot slot))
				slot.SetCollection(prefab);
		}
	}

	private List<GameObject> SortCollections(CollectionType type, List<GameObject> input)
	{
		List<GameObject> sortedList = new(input);

		switch (type)
		{
			case CollectionType.Enemy:
				sortedList.Sort((a, b) =>
				{
					if (a.TryGetComponent(out Enemy enemyA) && b.TryGetComponent(out Enemy enemyB))
						return enemyA.Strength.CompareTo(enemyB.Strength);
					return 0;
				});
				break;

			case CollectionType.Item:
				sortedList.Sort((a, b) =>
				{
					if (a.TryGetComponent(out Item itemA) && b.TryGetComponent(out Item itemB))
						return itemA.Rarity.CompareTo(itemB.Rarity);
					return 0;
				});
				break;

			case CollectionType.Tile:
				sortedList.Sort((a, b) =>
				{
					if (a.TryGetComponent(out Tile tileA) && b.TryGetComponent(out Tile tileB))
						return string.Compare(tileA.name, tileB.name, System.StringComparison.Ordinal);
					return 0;
				});
				break;
		}

		return sortedList;
	}

	public void OnPreviousButtonClick()
	{
		currentCollectionType = (CollectionType)(((int)currentCollectionType + 2) % 3);
		SpawnCollections(currentCollectionType);
	}

	public void OnNextButtonClick()
	{
		currentCollectionType = (CollectionType)(((int)currentCollectionType + 1) % 3);
		SpawnCollections(currentCollectionType);
	}
}
