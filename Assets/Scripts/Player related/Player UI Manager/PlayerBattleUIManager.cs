using Player_related.Player;
using SaintsField;
using UnityEngine;
using UnityEngine.UI;

namespace Player_related.Player_UI_Manager {
    public class PlayerBattleUIManager : MonoBehaviour
    {
        public Slider healthBar;
        public Image weaponIconImage;
        [SaintsDictionary("index", "weapon type")]
        public SaintsDictionary<int, Sprite> weaponIndexToWeaponIcon;

        public void UpdatePlayerHealth(Component component, object nothingHere)
        {
            PlayerStats playerStats = (PlayerStats)component;
            playerStats.UpdateHealthBar(this.healthBar);
        }

        public void UpdateWeaponIcon(Component component, object index)
        {
            this.weaponIconImage.sprite = this.weaponIndexToWeaponIcon[(int)((object[]) index)[0]];
        }
    }
}
