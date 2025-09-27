using System.Collections.Generic;
using System.Text;
using Common;
using Game.CharacterControls;
using GameplayAbilities.Runtime.Abilities;
using InteractionSystem.Runtime;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;
using WeaponsSystem;
using WeaponsSystem.DamageHandling;

namespace Game.Player {
    [DisallowMultipleComponent]
    public sealed class PlayerInputParser : MonoBehaviour {
        [field: SerializeField, Required] private Interactor Interactor { get; set; }
        [field: SerializeField, Required] private Inventory Inventory { get; set; }
        [field: SerializeField, Required] private InventoryUIManager InventoryUIManager { get; set; }
        [field: SerializeField, Required] private Movement Movement { get; set; }
        [field: SerializeField, Required] private SpriteAnimator Animator { get; set; }
        [field: SerializeField, Required] private Combatant Combatant { get; set; }
        [field: SerializeField, Required] private AbilityRoundRobin AbilityRoundRobin { get; set; }
        [field: SerializeField, Required] private AbilitySystem AbilitySystem { get; set; }
        
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
            this.Combatant.StartAttack();
            //this.MeleeWeapon.gameObject.SetActive(true);
            //this.RangedWeapon.gameObject.SetActive(true);
            
            //this.MeleeWeapon.Attack();
            //this.RangedWeapon.Attack();
        }

        public void OnMove(InputAction.CallbackContext context) {
            if (context.performed) {
                Vector2 input = context.ReadValue<Vector2>();
                this.Movement.MoveIn(input);
            } else if (context.canceled) {
                this.Movement.Stop();
            }
        }

        public void OnSkillOneUsed(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            } 
            
            Debug.Log($"Skill One: {this.Combatant.GetSkillOne()}");
            this.AbilitySystem.Use(this.Combatant.GetSkillOne(), this.AbilitySystem, this.AbilitySystem.CreateEffectExecutionArgs().Build());
        }

        public void OnUseAbility(InputAction.CallbackContext context) {
            if (context.performed) {
                this.AbilityRoundRobin.UseCurrentAbility();
            }
        }
    }
}
