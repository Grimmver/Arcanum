using TMPro;
using UI;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShopSell : MonoBehaviour
{
	public static ShopSell Instance { get; private set; }

	[SerializeField] private TMP_Text sellText;

	public int sellAmount = 1000;
	public int currentShopSold = 0;
	[SerializeField] private int maxSellPerShop = 1;

	private const string CRYSTAL_PREF = "Crystal";

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Item item))
		{
			item.SetOverShopSellZone(true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Item item))
		{
			item.SetOverShopSellZone(false);
		}
	}

	public void TrySell(Item item)
	{
		int crystal = PlayerPrefsEventBus.GetInt(CRYSTAL_PREF);

		if (!(item is MaskOfGreed mask) && crystal >= sellAmount && currentShopSold < maxSellPerShop)
		{

			if (item.CurrentSlot != null)
			{
				item.CurrentSlot.RemoveItem();
			}

			PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, crystal - sellAmount);
			currentShopSold++;

			item.Unequip();

			Destroy(item.gameObject);
			UpdateUI();
		}
		else
		{
			item.CancelDragging();
			item.transform.localPosition = Vector3.zero;
		}
	}

	public void UpdateUI()
	{
		string language = LocalizationEventBus.CurrentLanguage;
		bool isEN = language == "EN";

		if (currentShopSold < maxSellPerShop)
		{
			sellText.text = $"{(isEN ? "REMOVE ITEM:" : "УБРАТЬ ПРЕДМЕТ:")}\n{sellAmount}";
		}
		else
		{
			sellText.text = $"{(isEN ? "NOT AVAILABLE" : "НЕДОСТУПНО")}";
		}
	}
}
