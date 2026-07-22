using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ItemUI : MonoBehaviour
{
	[SerializeField] private TMP_Text itemNameText;
	[SerializeField] private TMP_Text itemDescriptionText;

	private bool isOpen;
	public bool IsOpen => isOpen;

	private Canvas canvas;
	private Item item;

	private void Awake()
	{
		canvas = GetComponent<Canvas>();
		item = GetComponentInParent<Item>();

		Open(false);
	}

	public void Open(bool action)
	{
		if (action)
		{
			Inventory.Instance.CloseItemUI();
			itemNameText.text = item.ItemName;
			SetNameColor();
			itemDescriptionText.text = item.Description;
		}

		canvas.enabled = action;
		isOpen = action;
	}

	private void SetNameColor()
	{
		switch (item.Rarity)
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
