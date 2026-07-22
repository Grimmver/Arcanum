using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class ShopItemSlot : MonoBehaviour, IPointerClickHandler
{
	[Header("UI")]
	[SerializeField] private TMP_Text costText;
	[SerializeField] private Image icon;
	//[SerializeField] private GameObject soldOverlay;

	private bool isSold = false;
	public bool IsSold => isSold;

	private Canvas canvas;

	public Item ItemPrefab { get; private set; }

	private void Awake()
	{
		canvas = GetComponent<Canvas>();
		//soldOverlay?.SetActive(false);
	}

	public void SetItem(Item itemPrefab)
	{
		ItemPrefab = itemPrefab;
		isSold = false;

		costText.text = itemPrefab.Cost.ToString();

		Sprite sprite = itemPrefab.GetComponentInChildren<SpriteRenderer>()?.sprite;
		if (sprite != null)
			icon.sprite = sprite;
		else
			Debug.LogWarning($"No SpriteRenderer found on {itemPrefab.name}");

		icon.color = Color.white;
		//soldOverlay?.SetActive(false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (isSold || ItemPrefab == null) return;

		ShopCanvas.Instance?.OpenShopPopup(ItemPrefab);
	}

	public void Sold()
	{
		isSold = true;
		icon.color = Color.gray;
		costText.text = "SOLD";
		//soldOverlay?.SetActive(true);
	}

	public void ResetSlot()
	{
		isSold = false;
		ItemPrefab = null;
		icon.color = Color.white;
		costText.text = "";
		//soldOverlay?.SetActive(false);
	}
}
