using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleUIManager : MonoBehaviour
{
    public Slider healthBar;

    public void UpdatePlayerHealth(Component component, object nothingHere)
    {
        PlayerStats playerStats = (PlayerStats)component;
        playerStats.UpdateHealthBar(healthBar);
    }
}
