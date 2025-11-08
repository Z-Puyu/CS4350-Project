using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Abilities;
using Player_related.Player_exp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skill_tree_related.Skill_tree_UI {
    public class SkillTreeUIManager : MonoBehaviour
    {
        [SerializeField] private PlayerExp PlayerExp;
        [SerializeField] private List<SkillLineManager> allSkillLineManager = new List<SkillLineManager>();
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
        [SerializeField] private Button canUnlockButton;
        [SerializeField] private GameObject sidebar;
        
        //Cache the skill icon result
        private SkillIcon skillIcon;
        
        void Start() {
            backdropPanel.SetActive(false);   
            canUnlockButton.gameObject.SetActive(false);
            sidebar.SetActive(false);
        }
        
        public void OpenBackdropPanel()
        {
            backdropPanel.SetActive(true);
            PlayerExp.UpdateExp(combatExpBar, farmingExpBar, numberOfCombatSkillPointText, numberOfFarmingSkillPointText);
            foreach (var skillLineManager in allSkillLineManager)
            {
                skillLineManager.InitialiseLine();
            }
        }

        public void SetSkillInformation(Component component, object skill)
        {
            skillIcon = (SkillIcon)component;
            Perk perk = (Perk)((object[])skill)[0];
            sidebar.SetActive(true);
            this.titleText.text = perk.Name;
            this.descriptionText.text = perk.Description;

            if (skillIcon.CheckIsUnlocked())
            {
                canUnlockButton.gameObject.SetActive(false);
            }
            else
            {
                canUnlockButton.gameObject.SetActive(PlayerExp.EnoughPoint(perk));
            }
        }

        public void UnlockSKill()
        {
            if (skillIcon == null) return;
            
            foreach (var skillLineManager in allSkillLineManager)
            {
                skillLineManager.LevelUp(skillIcon);
            }

            skillIcon = null;
            PlayerExp.UpdateExp(combatExpBar, farmingExpBar, numberOfCombatSkillPointText, numberOfFarmingSkillPointText);
        }
        
    }
}
