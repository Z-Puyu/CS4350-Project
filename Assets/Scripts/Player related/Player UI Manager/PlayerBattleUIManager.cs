using UnityEngine;
using UnityEngine.UI;
using SaintsField;

public class PlayerBattleUIManager : MonoBehaviour
{
    public Slider healthBar;
    public Image weaponIconImage;
    [SaintsDictionary("index", "weapon type")]
    public SaintsDictionary<int, Sprite> weaponIndexToWeaponIcon;

    public void UpdatePlayerHealth(Component component, object nothingHere)
    {
        PlayerStats playerStats = (PlayerStats)component;
        playerStats.UpdateHealthBar(healthBar);
    }

    public void UpdateWeaponIcon(Component component, object index)
    {
        weaponIconImage.sprite = weaponIndexToWeaponIcon[(int)((object[]) index)[0]];
    }
}
