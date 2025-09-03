using System.Collections.Generic;
using System.Text;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Player;
using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInput {
    [DisallowMultipleComponent]
    public sealed class PlayerInputParser : MonoBehaviour {
        [field: SerializeField, Required] private Inventory Inventory { get; set; }
        [field: SerializeField, Required] private PlayerMovement Movement { get; set; }
        
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
            if (context.performed) {
                Vector2 input = context.ReadValue<Vector2>();
                this.Movement.MoveTowards(input);
            } else if (context.canceled) {
                this.Movement.Stop();
            }
        }
    }
}
