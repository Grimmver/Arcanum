using UnityEngine;
using UnityEngine.UI;

namespace UI.Collections
{
	public class CollectionsSlot : MonoBehaviour
	{
		[SerializeField] private GameObject collection;
		public GameObject Collection => collection;

		[SerializeField] private Image icon;

		public void SetCollection(GameObject collection)
		{
			this.collection = collection;

			if (collection.TryGetComponent(out Item item))
			{
				icon.sprite = item.Sprite;
			}
			else if (collection.TryGetComponent(out Enemy enemy))
			{
				icon.sprite = enemy.Sprite;
			}
			else if (collection.TryGetComponent(out Tile tile))
			{
				icon.sprite = tile.Sprite;
			}
		}

		public void OnClick()
		{
			CollectionsPopup.Instance?.Open(collection);
		}
	}
}
