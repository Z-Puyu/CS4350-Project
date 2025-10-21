using System.Collections.Generic;
using Events;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

namespace Player_related.Player_quick_swap
{
    public class PlayerQuickSwapUIManager : MonoBehaviour
    {
        private Animator backdropAnimator;
        [SerializeField] private Inventory player_inventory;
        [SerializeField] private GameObject backdrop;
        [SerializeField] private List<QuickSwapIcon> allQuickSwapIcons;
        [SerializeField] private CrossObjectEventWithDataSO broadcastItemToUse;
        
        private int index = 0;
        
        void Start()
        {
            backdropAnimator = GetComponent<Animator>();
            allQuickSwapIcons[index].Select();
            CloseBackdrop();
        }

        public void EnableBackdrop()
        {
            backdropAnimator.SetBool("isOpen", true);
            backdrop.SetActive(true);
            UpdateQuickswapIcon();
        }

        public void UpdateQuickswapIcon()
        {
            int index = 0;
            foreach (QuickSwapIcon icon in allQuickSwapIcons)
            {
                icon.HideIcon();
            }
            foreach (string id in player_inventory.GetQuickSwapItems())
            {
                ItemData itemData;
                ItemDatabase.TryGet(id, out itemData);
                allQuickSwapIcons[index].SetIcon(itemData.Icon, player_inventory.Count(id));
                index++;
            }
        }
        
        public void StartClosingBackdrop()
        {
            foreach (QuickSwapIcon quickSwapIcon in allQuickSwapIcons)
            {
                quickSwapIcon.HideIcon();
            }
            backdropAnimator.SetBool("isOpen", false);
        }

        public void CloseBackdrop()
        {
            backdrop.SetActive(false);
        }

        public void ToggleSelection(float dir)
        {
            allQuickSwapIcons[index].Unselect();
            if (dir < 0)
            {
                index++;
                if (index >= allQuickSwapIcons.Count)
                {
                    index = 0;
                }
            }
            else
            {
                index--;
                if (index < 0)
                {
                    index = allQuickSwapIcons.Count - 1;
                }
            }
            allQuickSwapIcons[index].Select();
        }
        
        public void UseItem()
        {
            player_inventory.UseItem(index);
        }
        
    }
}
