using System.Collections.Generic;

public static class RarityChances
{
    public static Dictionary<ItemRarity, int> Weights = new()
    {
        { ItemRarity.Common, 55 },
        { ItemRarity.Rare, 24 },
        { ItemRarity.Epic, 15 },
        { ItemRarity.Legendary, 5 },
        { ItemRarity.Mythical, 1 }
    };
}
