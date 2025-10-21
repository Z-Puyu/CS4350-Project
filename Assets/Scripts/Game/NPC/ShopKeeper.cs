using Events;
using System.Runtime.InteropServices;
using Shop.Runtime;
using UnityEngine;

namespace Game.NPC
{
    public class ShopKeeper : MonoBehaviour
    {
        [field: SerializeField] private ShopKeeperData Data { get; set; }
        [field: SerializeField] private Animator Animator { get; set; }
        [field: SerializeField] private ShopInventory Inventory { get; set; }

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
    }

}
