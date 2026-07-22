using UnityEngine;
using TMPro;
using UI;

namespace MainMenu.Caravan
{
    public class ItemDescriptionCanvas : UICanvas<ItemDescriptionCanvas>
    {
        [SerializeField] private ItemData selectedItem;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text descriptionText;

        private void Start()
        {
            //Destroy(gameObject,);
        }

        public void OnCloseButtonClick() => Open(false);
    }
}
