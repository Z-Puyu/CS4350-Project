using System.Collections.Generic;
using System.Text;
using Common;
using Game.CharacterControls;
using InteractionSystem.Runtime;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Player_related.Player;
using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player {
    [DisallowMultipleComponent]
    public sealed class PlayerInputParser : MonoBehaviour {
        [field: SerializeField, Required] private Interactor Interactor { get; set; }
        [field: SerializeField, Required] private Inventory Inventory { get; set; }
        [field: SerializeField, Required] private InventoryUIManager InventoryUIManager { get; set; }
        [field: SerializeField, Required] private Movement Movement { get; set; }
        [field: SerializeField, Required] private SpriteAnimator Animator { get; set; }

        public void OnInteract(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
            OnScreenDebugger.Log("Interact");
            this.Interactor.Interact();
        }
        
        
        public void OnToggleInventory(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
            // Logging inventory items
            StringBuilder sb = new StringBuilder("Inventory:\n");
            foreach (KeyValuePair<ItemKey, int> item in this.Inventory) {
                sb.AppendLine($"{item.Key.Id}: {item.Value}");
            }
            
            OnScreenDebugger.Log(sb.ToString());
            
            bool isInventoryActive = this.InventoryUIManager.gameObject.activeSelf;
            
            // Toggle inventory UI
            this.InventoryUIManager.gameObject.SetActive(!isInventoryActive);
        }

        public void OnAttack(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
            OnScreenDebugger.Log("Attack");
            
            this.Animator.PlayAttack(); // Calls animator action
        }

        public void OnMove(InputAction.CallbackContext context) {
            if (context.performed) {
                Vector2 input = context.ReadValue<Vector2>();
                this.Movement.MoveIn(input);
            } else if (context.canceled) {
                this.Movement.Stop();
            }
        }
    }
}
