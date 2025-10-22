using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player_related.Player_quick_swap
{
    public class QuickSwapIcon : MonoBehaviour
    {
        public Sprite unselected;
        public Sprite selected;
        private Image imageRenderer;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemQuantity;

        private void Start()
        {
            itemIcon.gameObject.SetActive(false);
            itemQuantity.gameObject.SetActive(false);
            imageRenderer = GetComponent<Image>();
        }

        public void SetIcon(Sprite iconSprite, int quantity)
        {
            itemIcon.gameObject.SetActive(true);
            itemQuantity.gameObject.SetActive(true);
            itemIcon.sprite = iconSprite;
            itemQuantity.text = quantity.ToString();
        }

        public void HideIcon()
        {
            itemIcon.gameObject.SetActive(false);
            itemQuantity.gameObject.SetActive(false);
        }

        public void Select()
        {
            imageRenderer.sprite = selected;
        }

        public void Unselect()
        {
            imageRenderer.sprite = unselected;
        }
    }
}
