using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player_related.Player_quick_swap
{
    public class QuickSwapIcon : MonoBehaviour
    {
        public Sprite unselected;
        public Sprite selected;
        private Image imageRenderer;

        private void Start()
        {
            imageRenderer = GetComponent<Image>();
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
