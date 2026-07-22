using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MainMenu.Caravan
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ItemData itemData;

        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image itemImage;
        
        public void OnPointerClick(PointerEventData eventData)
        {

        }
    }
}
