using GameplayAbilities.Runtime.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skill_tree_related.Skill_tree_UI {
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
            this.UpdateExp();
        }

        public void SetSkillInformation(Component component, object skill)
        {
            Perk broadcastedSkill = (Perk)((object[])skill)[0];
            this.titleText.text = broadcastedSkill.Name;
            this.descriptionText.text = broadcastedSkill.Description;
        }

        public void UpdateExp()
        {
            this.combatExpBar.value = (((float)this.currentCombatExp) / ((float)this.maxCombatExp)) * 0.5f;
            this.farmingExpBar.value = (((float)this.currentFarmingExp) / ((float)this.maxFarmingExp)) * 0.5f;
        }


    }
}
