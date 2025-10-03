using GameplayAbilities.Runtime.HealthSystem;
using UnityEngine;
using UnityEngine.UI;
using SaintsField;

public class PlayerBattleUIManager : MonoBehaviour
{
    public Slider healthBar;
    public Health health;
    public Image weaponIconImage;
    [SaintsDictionary("index", "weapon type")]
    public SaintsDictionary<int, Sprite> weaponIndexToWeaponIcon;

    public void UpdateHealth()
    {
        healthBar.value = (float)health.Value/(float)health.MaxValue;
    }

    public void UpdateWeaponIcon(Component component, object index)
    {
        weaponIconImage.sprite = weaponIndexToWeaponIcon[(int)((object[]) index)[0]];
    }
}
