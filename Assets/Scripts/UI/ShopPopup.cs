using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ShopPopup : MonoBehaviour
	{
		[SerializeField] private Image itemIcon;
		[SerializeField] private TMP_Text itemNameText;
		[SerializeField] private TMP_Text itemCostText;
		[SerializeField] private TMP_Text itemDescriptionText;
		[SerializeField] private TMP_Text itemBuyText;

		private Item selectedItem;
		private const string CRYSTAL_PREF = "Crystal";

		public event Action<Item> OnSuccessfulBuy;

		private void Start()
		{
			UpdateUI();
		}

		public void Open(bool action)
		{
			Inventory.Instance?.CancelAllDraggingItems();

			if (action && Inventory.Instance != null)
			{
				foreach (var slot in Inventory.Instance.ItemSlots)
				{
					if (slot.EquippedItem != null)
					{
						slot.EquippedItem.transform.position = slot.transform.position;
						slot.EquippedItem.transform.localPosition = Vector3.zero;
					}
				}
			}

			gameObject.SetActive(action);
			UpdateUI();
		}

		public void OnBuyButtonClick()
		{
			if (selectedItem == null || Inventory.Instance == null) return;

			int crystal = PlayerPrefsEventBus.GetInt(CRYSTAL_PREF, 100);
			if (crystal < selectedItem.Cost)
			{
				Debug.Log("Not enough Crystals.");
				return;
			}

			GameObject instance = Instantiate(selectedItem.gameObject);
			if (instance.TryGetComponent(out Item itemInstance))
			{
				if (Inventory.Instance.TryAddItem(itemInstance))
				{
					PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, crystal - selectedItem.Cost);
					instance.transform.localScale = selectedItem.transform.localScale;
					OnSuccessfulBuy?.Invoke(selectedItem);
				}
				else
				{
					Destroy(instance);
					Debug.Log("Inventory is full.");
				}
			}
			else
			{
				Destroy(instance);
				Debug.LogWarning("Selected prefab is not a valid Item.");
			}
		}

		public void OnBackButtonClick() => ShopCanvas.Instance?.CloseShopPopup();

		public void SetItem(Item item)
		{
			selectedItem = item;
			UpdateUI();
		}

		private void UpdateUI()
		{
			if (selectedItem == null) return;

			Sprite iconSprite = selectedItem.GetComponentInChildren<SpriteRenderer>()?.sprite;
			itemIcon.sprite = iconSprite;
			itemNameText.text = selectedItem.ItemName;
			SetNameColor();
			itemCostText.text = selectedItem.Cost.ToString();
			itemDescriptionText.text = selectedItem.Description;

			int crystal = PlayerPrefsEventBus.GetInt(CRYSTAL_PREF, 0);
			bool canAfford = (crystal >= selectedItem.Cost) && (Inventory.Instance?.IsInventoryFull() == false);
			itemBuyText.color = canAfford ? Color.green : Color.gray;
		}

		private void SetNameColor()
		{
			switch (selectedItem.Rarity)
			{
				case ItemRarity.Common:
					itemNameText.color = Color.white;
					break;
				case ItemRarity.Rare:
					itemNameText.color = Color.orange;
					break;
				case ItemRarity.Epic:
					itemNameText.color = Color.purple;
					break;
				case ItemRarity.Legendary:
					itemNameText.color = Color.red;
					break;
				case ItemRarity.Mythical:
					itemNameText.color = Color.cyan;
					break;
			}
		}
	}
}
