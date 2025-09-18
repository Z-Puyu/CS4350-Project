using UnityEngine;
using UnityEngine.InputSystem;
using Events;

public class PlayerSwitchWeapon : MonoBehaviour
{
    public CrossObjectEventWithDataSO broadcastSwitchWeapon;
    private int currentWeaponIndex = 0;

    void OnSwitchMelee(InputValue button)
    {
        currentWeaponIndex = 0;
        BroadcastWeaponSwitched();
    }

    void OnSwitchRanged(InputValue button)
    {
        currentWeaponIndex = 1;
        BroadcastWeaponSwitched();
    }

    void OnSwitchPlaceable(InputValue button)
    {
        currentWeaponIndex = 2;
        BroadcastWeaponSwitched();
    }

    void BroadcastWeaponSwitched()
    {
        broadcastSwitchWeapon.TriggerEvent(this, currentWeaponIndex);
    }
}
