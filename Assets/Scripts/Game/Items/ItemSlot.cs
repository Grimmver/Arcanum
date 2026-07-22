using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemSlot : MonoBehaviour
{
	[SerializeField] private Item equippedItem;
	public Item EquippedItem => equippedItem;

	private void Start()
	{
		if (transform.childCount > 0 && transform.GetChild(0).TryGetComponent(out Item item))
		{
			if (item.CurrentSlot == null)
			{
                Inventory.Instance?.TryAddItem(item);
                AssignItem(item);
            }
            
			//AssignItem(item);
			//item.Equip(Player.Instance);
		}
	}

	public bool CanAcceptItem(Item item) => equippedItem == null;

	public void AssignItem(Item item)
	{
		if (item.CurrentSlot != null && item.CurrentSlot != this)
		{
			item.CurrentSlot.RemoveItem();
		}

		equippedItem = item;
		item.CurrentSlot = this;

        item.transform.SetParent(transform);
        //item.transform.localPosition = Vector3.zero;
        //item.transform.localRotation = Quaternion.identity;
        //item.transform.localScale = Vector3.one;
        item.transform.localPosition = new Vector3(0, 0, -0.1f);
    }

	public void RemoveItem()
	{
		if (equippedItem != null)
		{
			equippedItem.transform.SetParent(null);
			equippedItem.CurrentSlot = null;

			equippedItem = null;
		}
	}
}
