using Events;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Shop.Runtime;
using UnityEngine;
using Shop_related.Shop_UI_Manager;
using WeaponsSystem.Runtime.WeaponComponents;

namespace Game.NPC
{
    public class Blacksmith : MonoBehaviour
    {
        [Header("Blacksmith Setup")]
        [field: SerializeField] private BlackSmithData Data { get; set; }
        [field: SerializeField] private Animator Animator { get; set; }
        [Header("Blacksmith Inventory")]
        [field: SerializeField] private BlacksmithInventory Inventory { get; set; }//NEED TO CHANGE THIS

        [Header("Blacksmith UI Event")]
        [SerializeField] private CrossObjectEventWithDataSO broadcastOpenShopUI;
        [SerializeField] private BlacksmithUIManager blacksmithUIManager;//NEED TO CHANGE THIS

        private bool hasInteracted = false;

        protected void Start()
        {
            if (!this.Data)
            {
                return;
            }

            if (this.Animator)
            {
                this.Animator.runtimeAnimatorController = this.Data.Animations;
            }
            this.Inventory.Use(new List<WeaponComponent>(this.Data.Items.Keys));
        }
        public void Interact()
        {

            if (hasInteracted)
                return;

            hasInteracted = true;

            if (!Inventory)
            {
                Debug.LogWarning("[Blacksmith] No Inventory assigned.", this);
                return;
            }
            OpenShopUI();
        }

        public void ResetInteraction()
        {
            hasInteracted = false;
        }
        public void OpenShopUI()
        {
            if (blacksmithUIManager == null)
            {
                Debug.LogWarning("[Blacksmith] No blacksmithUIManager assigned!", this);
                return;
            }
            
            blacksmithUIManager.gameObject.SetActive(true);
            blacksmithUIManager.SetBlacksmithInventory(this.Inventory);

        }
    }
}