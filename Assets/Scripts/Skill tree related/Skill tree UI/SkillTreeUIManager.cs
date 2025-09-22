using GameplayAbilities.Runtime.Abilities;
using Skill_tree_related.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUIManager : MonoBehaviour
{
    public Slider combatExpBar;
    public Slider farmingExpBar;
    [SerializeField]
    private int currentCombatExp;
    [SerializeField]
    private int currentFarmingExp;
    [SerializeField]
    private int maxCombatExp;
    [SerializeField]
    private int maxFarmingExp;
    [Header("Skill tab")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    
    void Start()
    {
        UpdateExp();
    }

    public void SetSkillInformation(Component component, object skill)
    {
        Perk broasdcastedSkill = (Perk)((object[])skill)[0];
        titleText.text = broasdcastedSkill.Name;
        descriptionText.text = broasdcastedSkill.Description;
    }

    public void UpdateExp()
    {
        combatExpBar.value = (((float)currentCombatExp) / ((float)maxCombatExp)) * 0.5f;
        farmingExpBar.value = (((float)currentFarmingExp) / ((float)maxFarmingExp)) * 0.5f;
    }


}
