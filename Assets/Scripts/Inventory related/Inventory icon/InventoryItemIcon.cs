using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Inventory_related.Inventory_icon {
    public class InventoryItemIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Image image;
        public TextMeshProUGUI bonusText;
        public GameObject hoverOverScrollView;

        void Start()
        {
            this.hoverOverScrollView.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            Vector2 mousePos = this.transform.InverseTransformPoint(Mouse.current.position.ReadValue());
            this.hoverOverScrollView.transform.localPosition = new Vector2(mousePos.x + 100, mousePos.y - 150);
            this.hoverOverScrollView.SetActive(true);
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            this.hoverOverScrollView.SetActive(false);
        }
    }
}
