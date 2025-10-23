using Events;
using System.Runtime.InteropServices;
using Shop.Runtime;
using UnityEngine;
using Shop_related.Shop_UI_Manager;

namespace Game.NPC
{
    public class ShopKeeper : MonoBehaviour
    {
        [Header("Shopkeeper Setup")]
        [field: SerializeField] private ShopKeeperData Data { get; set; }
        [field: SerializeField] private Animator Animator { get; set; }
        [Header("Shop Inventory")]
        [field: SerializeField] private ShopInventory Inventory { get; set; }

        [Header("Shop UI Event")]
        [SerializeField] private CrossObjectEventWithDataSO broadcastOpenShopUI;
        [SerializeField] private ShopUIManager shopUIManager;

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
            this.Inventory.Use(this.Data.Items);
        }
        public void Interact()
        {

            if (hasInteracted)
                return;

            hasInteracted = true;

            if (!Inventory)
            {
                Debug.LogWarning("[ShopKeeper] No Inventory assigned.", this);
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
            if (shopUIManager == null)
            {
                Debug.LogWarning("[ShopKeeper] No ShopUIManager assigned!", this);
                return;
            }

            Debug.Log("[ShopKeeper] OpenShopUI method called!", this);

            shopUIManager.gameObject.SetActive(true);
            shopUIManager.SetShopInventory(this.Inventory);

            Debug.Log("[ShopKeeper] Shop UI opened via ShopUIManager.");
        }
    }

}
