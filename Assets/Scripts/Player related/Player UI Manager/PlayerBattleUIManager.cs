using System.Collections;
using GameplayAbilities.Runtime.HealthSystem;
using GameplayAbilities.Runtime.StaminaSystem;
using UnityEngine;
using UnityEngine.UI;
using SaintsField;

public class PlayerBattleUIManager : MonoBehaviour
{
    public Slider healthBar;
    public Health health;
    public Slider staminaBar;
    public Stamina stamina;
    public Image weaponIconImage;
    public GameObject objectiveUnlockedPromptIndicator;
    [SaintsDictionary("index", "weapon type")]
    public SaintsDictionary<int, Sprite> weaponIndexToWeaponIcon;

    void Awake()
    {
        objectiveUnlockedPromptIndicator.SetActive(false);
    }

    void Update()
    {
        healthBar.value = (float)health.Value/(float)health.MaxValue;
        staminaBar.value = (float)stamina.Value/(float)stamina.MaxValue;
    }

    public void UpdateWeaponIcon(int index)
    {
        weaponIconImage.sprite = weaponIndexToWeaponIcon[index];
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
