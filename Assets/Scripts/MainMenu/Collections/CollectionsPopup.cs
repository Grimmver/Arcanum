using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Collections
{
	public class CollectionsPopup : MonoBehaviour
	{
		public static CollectionsPopup Instance { get; private set; }

		[Header("General")]
		[SerializeField] private GameObject selectedCollection;
		[SerializeField] private Image darkBackground;
		[SerializeField] private GameObject panel;

		[Header("Collection")]
		[SerializeField] private Image collectionIcon;
		[SerializeField] private TMP_Text collectionNameText;
		[SerializeField] private TMP_Text collectionMainInfoText;
		[SerializeField] private TMP_Text collectionDescriptionText;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		private void Start()
		{
			panel.SetActive(false);
		}

		public void Open(GameObject collection)
		{
			SetCollection(collection);

			panel.SetActive(true);
			darkBackground.enabled = true;
		}

		public void OnBackButtonClick()
		{
			panel.SetActive(false);
			darkBackground.enabled = false;
		}

		public void SetCollection(GameObject collection)
		{
			if (collection == null) return;

			selectedCollection = collection;

			if (collection.TryGetComponent(out Enemy enemy))
			{
				SetEnemyData(enemy);
			}
			else if (collection.TryGetComponent(out Item item))
			{
				SetItemData(item);
			}
			else if (collection.TryGetComponent(out Tile tile))
			{
				SetTileData(tile);
			}
			else
			{
				Debug.LogWarning("Unknown collection type");
			}
		}

		private void SetEnemyData(Enemy enemy)
		{
			collectionIcon.sprite = enemy.Sprite;
			collectionNameText.text = enemy.Name;

			string language = LocalizationEventBus.CurrentLanguage;
			bool isEN = language == "EN";
			collectionMainInfoText.text =
				$"{(isEN ? "Type" : "Тип")}: <color=#ff4d4d>{(isEN ? "Enemy" : "Враг")}</color>\n" +
				$"{(isEN ? "Class" : "Класс")}: {EnemyTypeUtility.GetEnemyCategoryName(enemy)}\n" +
				$"{(isEN ? "Strength" : "Сила")}: {enemy.Strength}\n" +
				$"{(isEN ? "Stats" : "Характеристики")}: {enemy.MinAttackDamage}-{enemy.MaxAttackDamage} {(isEN ? "damage" : "урона")} / {enemy.MaxHealth} {(isEN ? "health" : "здоровья")}\n" +
				$"{(isEN ? "Scale" : "Коэфицент Роста")}: {enemy.MultiplierPerLeftStrength}";

			collectionDescriptionText.text = enemy.Description;

			if (enemy.ResistenceStats.Count > 0)
			{
				collectionDescriptionText.text += $"\n\n<color=#88F>{(isEN ? "Resistances:" : "Сопротивления:")}</color>";

				foreach (ResistenceStat resistance in enemy.ResistenceStats)
				{
					float percentage = (1f - resistance.resistenceCoefficient) * 100f;
					if (percentage == 0f) continue;

					string typeName = resistance.resistenceDamageType.ToString();
					string sign = percentage > 0 ? "+" : "-";
					string color = percentage > 0 ? "#6F6" : "#F66";

					collectionDescriptionText.text += $"\n<color={color}>{sign} {Mathf.Abs(percentage):0.#}% {typeName} {(isEN ? "Resistance" : "сопротивления")}</color>";
				}
			}
		}

		private void SetItemData(Item item)
		{
			collectionIcon.sprite = item.Sprite;
			collectionNameText.text = item.ItemName;

			Color rarityColor;
			switch (item.Rarity)
			{
				case ItemRarity.Common:
					rarityColor = Color.white;
					break;
				case ItemRarity.Rare:
					rarityColor = new Color(1f, 0.5f, 0f);
					break;
				case ItemRarity.Epic:
					rarityColor = new Color(0.6f, 0f, 1f);
					break;
				case ItemRarity.Legendary:
					rarityColor = Color.red;
					break;
				case ItemRarity.Mythical:
					rarityColor = Color.cyan;
					break;
				default:
					rarityColor = Color.white;
					break;
			}

			Color32 c = rarityColor;
			string rarityHex = $"#{c.r:X2}{c.g:X2}{c.b:X2}";
			string rarityText = LocalizationUtility.GetLocalizedRarity(item.Rarity);
			string coloredRarity = $"<color={rarityHex}>{rarityText}</color>";

			string language = LocalizationEventBus.CurrentLanguage;
			bool isEN = language == "EN";
			collectionMainInfoText.text = $"{(isEN ? "Type" : "Тип")}: {(isEN ? "Item" : "Предмет")}\n{(isEN ? "Rarity" : "Редкость")}: {coloredRarity}\n{(isEN ? "Cost" : "Цена")}: {item.Cost}";
			collectionDescriptionText.text = item.Description;
		}

		private void SetTileData(Tile tile)
		{
			collectionIcon.sprite = tile.Sprite;
			collectionNameText.text = tile.Name;

			string language = LocalizationEventBus.CurrentLanguage;
			bool isEN = language == "EN";
			string type = tile.Name;
			string role = tile.IsPrimarilyOffensive() ? (isEN ? "<color=red>Offensive</color>" : "<color=red>Атакующий</color>") : (isEN ? "<color=blue>Defensive</color>" : "<color=blue>Защитный</color>");

			collectionMainInfoText.text =
				$"{(isEN ? "Type" : "Тип")}: {(isEN ? "Element" : "Элемент")}\n{(isEN ? "Element" : "Элемент")}: {type}\n{(isEN ? "Role" : "Роль")}: {role}";

			collectionDescriptionText.text = tile.Description;
		}
	}
}
