using System.Collections;
using GameplayAbilities.Runtime.HealthSystem;
using UnityEngine;
using UnityEngine.UI;
using SaintsField;

public class PlayerBattleUIManager : MonoBehaviour
{
    public Slider healthBar;
    public Health health;
    public Image weaponIconImage;
    public GameObject objectiveUnlockedPromptIndicator;
    [SaintsDictionary("index", "weapon type")]
    public SaintsDictionary<int, Sprite> weaponIndexToWeaponIcon;

    void Awake()
    {
        objectiveUnlockedPromptIndicator.SetActive(false);
    }
    
    public void UpdateHealth()
    {
        healthBar.value = (float)health.Value/(float)health.MaxValue;
    }

    public void UpdateWeaponIcon(Component component, object index)
    {
        weaponIconImage.sprite = weaponIndexToWeaponIcon[(int)((object[]) index)[0]];
    }

    public void ShowObjectiveUnlockPromptIndicator()
    {
        StartCoroutine(ShowObjectiveUnlockPromptIndicatorCoroutine());
    }

    IEnumerator ShowObjectiveUnlockPromptIndicatorCoroutine()
    {
        objectiveUnlockedPromptIndicator.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        objectiveUnlockedPromptIndicator.SetActive(false);
    }
}
