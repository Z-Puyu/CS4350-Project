using System.Collections;
using System.Collections.Generic;
using System.Text;
using Common;
using Events;
using Game.CharacterControls;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Targeting;
using InteractionSystem.Runtime;
using Inventory_related.Inventory_UI_Manager_V2;
using Inventory_related.Inventory_UI_Manager;
using ModularItemsAndInventory.Runtime.Inventory;
using ModularItemsAndInventory.Runtime.Items;
using Player_related.Player_quick_swap;
using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;
using WeaponsSystem.Runtime.Combat;
using WeaponsSystem.Runtime.Equipments;

//using WeaponsSystem;
//using WeaponsSystem.DamageHandling;

namespace Game.Player {
    [DisallowMultipleComponent]
    public sealed class PlayerInputParser : MonoBehaviour {
        [field: SerializeField, Required] private Interactor Interactor { get; set; }
        [field: SerializeField, Required] private Inventory Inventory { get; set; }
        [field: SerializeField, Required] private InventoryUIManager InventoryUIManager { get; set; }
        [field: SerializeField, Required] private InventoryUIManagerV2 InventoryUIManagerV2 { get; set; }
        [field: SerializeField, Required] private Movement Movement { get; set; }
        [field: SerializeField, Required] private FarmerSpriteAnimator Animator { get; set; }
        [field: SerializeField, Required] private Combatant Combatant { get; set; }
        [field: SerializeField, Required] private AbilityCaster AbilityCaster { get; set; }
        [field: SerializeField, Required] private AbilityTargeter AbilityTargeter { get; set; }
        [field: SerializeField, Required] private AbilitySystem AbilitySystem { get; set; }
        [field: SerializeField, Required] private Weaponry Weaponry { get; set; }
        [field: SerializeField, Required] private CrossObjectEventSO broadcastOpenNotebook { get; set; }
        [field: SerializeField, Required] private CrossObjectEventSO broadcastPauseGame { get; set; }
        [field: SerializeField, Required] private PlayerQuickSwapUIManager PlayerQuickSwapUIManager { get; set; }
        [field: SerializeField, Required] private PlayerBattleUIManager PlayerBattleUIManager { get; set; }
        
        [field: SerializeField, Required] private GameplayAbilities.Runtime.StaminaSystem.Stamina Stamina { get; set; }
        
        private bool isQuickSwap = false;
        private Vector2 currentMoveInput = Vector2.zero; // store the latest WASD input
        
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

            bool isInventoryActive = this.InventoryUIManagerV2.gameObject.activeSelf;

            // Toggle inventory UI
            this.InventoryUIManagerV2.gameObject.SetActive(!isInventoryActive);
        }

        public void OnAttack(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            OnScreenDebugger.Log("Attack");
            if (this.AbilityTargeter.IsTargting) {
                this.AbilityTargeter.Confirm();
            } else {
                this.Combatant.StartAttack();
            }
        }

        public void OnOpenObjective(InputAction.CallbackContext context)
        {
            if (context.performed) {
                broadcastOpenNotebook.TriggerEvent();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                currentMoveInput = context.ReadValue<Vector2>();
                Movement.MoveIn(currentMoveInput);
            }
            else if (context.canceled)
            {
                currentMoveInput = Vector2.zero;
                Movement.Stop();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                broadcastPauseGame.TriggerEvent();
            }
        }

        public void OnUseWeaponSkill(InputAction.CallbackContext context) {
            if (context.performed) {
#if DEBUG
                Debug.Log("Use weapon skill");
#endif
                if (this.AbilityCaster.ReadiedSkillIndex == 0) {
                    this.AbilityTargeter.Cancel();
                }

                this.AbilityCaster.Ready(0);
            }
        }

        public void OnUseCharacterSkill(InputAction.CallbackContext context) {
            if (context.performed) {
#if DEBUG
                Debug.Log("Use character skill");
#endif
                if (this.AbilityCaster.ReadiedSkillIndex == 1) {
                    this.AbilityTargeter.Cancel();
                }

                this.AbilityCaster.Ready(1);
            }
        }

        public void OnSwitchToFirst(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            bool canSwitch = this.Weaponry.Switch(0);
            if (canSwitch)
            {
                PlayerBattleUIManager.UpdateWeaponIcon(0);
            }
        }

        public void OnSwitchToSecond(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            bool canSwitch = this.Weaponry.Switch(1);
            if (canSwitch)
            {
                PlayerBattleUIManager.UpdateWeaponIcon(1);
            }
        }

        public void OnSwitchToThird(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            bool canSwitch = this.Weaponry.Switch(2);
            if (canSwitch)
            {
                PlayerBattleUIManager.UpdateWeaponIcon(2);
            }
        }
        
        public void OnQuickSwapPage(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isQuickSwap = true;
                PlayerQuickSwapUIManager.EnableBackdrop();
            }
            else if (context.canceled)
            {
                isQuickSwap = false;
                PlayerQuickSwapUIManager.StartClosingBackdrop();
            }
        }
        
        public void OnToggleQuickSwap(InputAction.CallbackContext context)
        {
            if (context.performed && isQuickSwap)
            {
                Vector2 scrollDir = context.ReadValue<Vector2>();
                PlayerQuickSwapUIManager.ToggleSelection(scrollDir.y);
            }
        }
        

        public void OnToggleQuickConsume(InputAction.CallbackContext context)
        {
            if (context.performed && isQuickSwap)
            {
                PlayerQuickSwapUIManager.UseItem();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            const int dashCost = 20;

            if (!Stamina.HasEnough(dashCost))
            {
                OnScreenDebugger.Log("Not enough stamina to dash.");
                return;
            }

            Stamina.Consume(dashCost);

            // Dash direction
            Vector2 dashDir = currentMoveInput.sqrMagnitude > 0.01f 
                ? currentMoveInput 
                : Movement.GetLastMoveDirection();

            Animator.LastDashDirection = dashDir;

            // Start dash
            Movement.Dash(dashDir);
        }
    }
}
