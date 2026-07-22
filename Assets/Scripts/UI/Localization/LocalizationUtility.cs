using System.Collections.Generic;
using UI;

public static class LocalizationUtility
{
	private const string LANGUAGE_PREF = "Language";

	private static readonly Dictionary<CollectionType, LocalizedTextBlock> _localizedCollectionTypes = new()
	{
		{
			CollectionType.Enemy, new LocalizedTextBlock
			{
				languages = new()
				{
					new LocalizedTextEntry { Language = "EN", Text = "Enemies" },
					new LocalizedTextEntry { Language = "RU", Text = "Противники" },
				}
			}
		},
		{
			CollectionType.Item, new LocalizedTextBlock
			{
				languages = new()
				{
					new LocalizedTextEntry { Language = "EN", Text = "Items" },
					new LocalizedTextEntry { Language = "RU", Text = "Предметы" },
				}
			}
		},
		{
			CollectionType.Tile, new LocalizedTextBlock
			{
				languages = new()
				{
					new LocalizedTextEntry { Language = "EN", Text = "Elements" },
					new LocalizedTextEntry { Language = "RU", Text = "Элементы" },
				}
			}
		}
	};

	public static string GetLocalizedCollectionType(CollectionType type)
	{
		if (_localizedCollectionTypes.TryGetValue(type, out var block))
			return block.Get();

		return type.ToString();
	}

	public static string GetLocalizedRarity(ItemRarity rarity)
	{
		string language = PlayerPrefsEventBus.GetString(LANGUAGE_PREF);
		bool isEN = language == "EN";

		return rarity switch
		{
			ItemRarity.Common => isEN ? "Common" : "Обычный",
			ItemRarity.Rare => isEN ? "Rare" : "Редкий",
			ItemRarity.Epic => isEN ? "Epic" : "Эпический",
			ItemRarity.Legendary => isEN ? "Legendary" : "Легендарный",
			ItemRarity.Mythical => isEN ? "Mythical" : "Мифический",
			_ => isEN ? "Unknown" : "Неизвестный"
		};
	}
}
