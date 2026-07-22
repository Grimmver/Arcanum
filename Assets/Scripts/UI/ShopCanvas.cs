using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

namespace UI
{
	[RequireComponent(typeof(Canvas))]
	public class ShopCanvas : UICanvas<ShopCanvas>
	{
		[Header("General")]
		[SerializeField] private ShopPopup shopPopup;
		[SerializeField] private Shop shop;
		[SerializeField] private GameObject darkBackground;
		private bool isShopPopupOpen;
		public bool IsShopPopupOpen => isShopPopupOpen;

		[Header("Reroll")]
		[SerializeField] private TMP_Text rerollCostText;
		[SerializeField] private Button rerollButton;

		[Header("Healing")]
		[SerializeField] private TMP_Text healingCostText;
		[SerializeField] private Image healingIcon;

        [Header("Remove item")]
        [SerializeField] private TMP_Text removeCostText;
        [SerializeField] private Image removeIcon;

        private FloatingText[] disabledFloatingTexts = new FloatingText[1];

		private void Start()
		{
			canvas.enabled = false;
			UpdateRerollUI();
		}

		private void OnEnable()
		{
			shopPopup.OnSuccessfulBuy += OnSuccessfulBuy;
		}

		private void OnDisable()
		{
			shopPopup.OnSuccessfulBuy -= OnSuccessfulBuy;
		}

		public void OpenShopPopup(Item itemPrefab)
		{
			if (itemPrefab == null) return;

			shopPopup.SetItem(itemPrefab);
			shopPopup.Open(true);
			darkBackground?.SetActive(true);
		}

		public void CloseShopPopup()
		{
			shopPopup.Open(false);
			darkBackground?.SetActive(false);
		}

		public void OnRerollButtonClick()
		{
			Shop.Instance?.Reroll();
			UpdateRerollUI();
		}

		public override void Open(bool action)
		{
			base.Open(action);
			shop.Open(action);
			// Shop.Instance?.Open(action);
			UpdateRerollUI();

			if (action)
			{
				disabledFloatingTexts = Object.FindObjectsByType<FloatingText>(FindObjectsSortMode.None);
			}
			else
			{
                EnemyWaveManager.Instance.CheckAndSpawnNextWave();
            }

			foreach (FloatingText floatingText in disabledFloatingTexts)
			{
				if (floatingText != null)
				{
					floatingText.gameObject.SetActive(!action);
				}
			}
		}

		private void UpdateRerollUI()
		{
			if (Shop.Instance == null) return;

			bool allSold = Shop.Instance.AreAllSlotsSold();
			rerollButton.interactable = !allSold;
			rerollCostText.text = allSold ? "-" : Shop.Instance.rerollCost.ToString();
		}

		private void OnSuccessfulBuy(Item boughtItem)
		{
			CloseShopPopup();
			Shop.Instance?.HandleItemSold(boughtItem);
			UpdateRerollUI();
		}

		public void UpdateHealingUI(string displayText, Color color)
		{
			healingCostText.text = displayText;
			healingIcon.color = color;
		}

        public void UpdateRemoveUI(string displayText, Color color)
        {
            removeCostText.text = displayText;
            removeIcon.color = color;
        }
    }
}
