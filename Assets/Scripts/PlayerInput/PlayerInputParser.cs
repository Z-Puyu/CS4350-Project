using System.Collections.Generic;
using System.Text;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInput {
    [DisallowMultipleComponent]
    public sealed class PlayerInputParser : MonoBehaviour {
        [field: SerializeField] private Inventory Inventory { get; set; }

        public void OnToggleInventory(InputAction.CallbackContext context) {
            StringBuilder sb = new StringBuilder("Inventory:\n");
            foreach (KeyValuePair<ItemKey, int> item in this.Inventory) {
                sb.AppendLine($"{item.Key}: {item.Value}");
            }
            
            Debug.Log(sb, this);
        }

        public void OnAttack(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
            Debug.Log("Attack", this);
        }

        public void OnMove(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
            Vector2 input = context.ReadValue<Vector2>();
            Debug.Log($"Move towards {input}", this);
        }
    }
}
