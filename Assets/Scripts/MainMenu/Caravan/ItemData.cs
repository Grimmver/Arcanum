using UnityEngine;

namespace MainMenu.Caravan
{
    public class ItemData : ScriptableObject
    {
        [SerializeField] protected string itemName;
        public string Name => itemName;

        [SerializeField] protected int cost;
        public int Cost => cost;

        [SerializeField] [TextArea(5, 7)] protected string description;
        public string Description => description;

        [SerializeField] protected Sprite icon;
        public Sprite Icon => icon;
    }
}
