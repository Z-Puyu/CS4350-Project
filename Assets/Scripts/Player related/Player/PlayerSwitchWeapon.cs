using Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player_related.Player {
    public class PlayerSwitchWeapon : MonoBehaviour
    {
        public CrossObjectEventWithDataSO broadcastSwitchWeapon;
        private int currentWeaponIndex = 0;

        void OnSwitchMelee(InputValue button)
        {
            this.currentWeaponIndex = 0;
            this.BroadcastWeaponSwitched();
        }

        void OnSwitchRanged(InputValue button)
        {
            this.currentWeaponIndex = 1;
            this.BroadcastWeaponSwitched();
        }

        void OnSwitchPlaceable(InputValue button)
        {
            this.currentWeaponIndex = 2;
            this.BroadcastWeaponSwitched();
        }

        void BroadcastWeaponSwitched()
        {
            this.broadcastSwitchWeapon.TriggerEvent(this, this.currentWeaponIndex);
        }
    }
}
