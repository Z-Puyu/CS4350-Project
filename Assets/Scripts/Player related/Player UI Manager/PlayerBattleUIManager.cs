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
        health.OnHealthChanged += UpdateHealth;
        stamina.OnStaminaChanged += UpdateStamina;
    }
    
    void UpdateHealth((int old, int curr) data)
    {
        healthBar.value = (float)data.curr/(float)health.MaxValue;
    }

    public void UpdateStamina((int old, int curr) data)
    {
        staminaBar.value = (float)stamina.Value/(float)stamina.MaxValue;
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
