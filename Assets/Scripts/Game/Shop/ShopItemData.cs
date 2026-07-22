using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items/Shop/Shop Item", fileName = "Shop Item")]
public class ShopItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    public string Name => itemName;

    [SerializeField] private ItemRarity rarity;
    public ItemRarity Rarity => rarity;

    [SerializeField] [Min(0)] private int cost;
    public int Cost => cost;

    [SerializeField] [TextArea(3, 10)] private string itemDescription;
    public string Description => itemDescription;
    
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    [SerializeField] private GameObject itemPrefab;
    public GameObject ItemPrefab => itemPrefab;
}
