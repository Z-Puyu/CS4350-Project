using System.Collections.Generic;
using UnityEngine;

namespace Player_related.Player_quick_swap
{
    public class PlayerQuickSwapUIManager : MonoBehaviour
    {
        private Animator backdropAnimator;
        [SerializeField] private GameObject backdrop;
        [SerializeField] private List<QuickSwapIcon> allQuickSwapIcons;
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
        }
        
        public void StartClosingBackdrop()
        {
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
        
    }
}
