using GameplayAbilities.Runtime.Abilities;
using Player_related.Player_exp;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Skill_tree_related.Skill_tree_UI {
    public class SkillTreeUIManager : MonoBehaviour
    {
        [SerializeField] private PlayerExp PlayerExp;
        [SerializeField]
        private GameObject backdropPanel;
        
        [Header("Central skill icon")]
        [SerializeField] private Slider combatExpBar;
        [SerializeField] private Slider farmingExpBar;
        [SerializeField] private TextMeshProUGUI numberOfCombatSkillPointText;
        [SerializeField] private TextMeshProUGUI numberOfFarmingSkillPointText;
        
        [Header("Skill tab")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        public void OpenBackdropPanel()
        {
            backdropPanel.SetActive(true);
            PlayerExp.UpdateExp(combatExpBar, farmingExpBar, numberOfCombatSkillPointText, numberOfFarmingSkillPointText);
        }

        public void SetSkillInformation(Component component, object skill)
        {
            Perk broadcastedSkill = (Perk)((object[])skill)[0];
            this.titleText.text = broadcastedSkill.Name;
            this.descriptionText.text = broadcastedSkill.Description;
        }
        
    }
}
