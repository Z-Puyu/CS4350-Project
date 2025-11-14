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
        [SerializeField] private TextMeshProUGUI skillPointNeedText;
        [SerializeField] private Button canUnlockButton;
        private TextMeshProUGUI canUnlockButtonText;
        [SerializeField] private GameObject sidebar;
        
        //Cache the skill icon result
        private SkillIcon skillIcon;
        
        void Start()
        {
            canUnlockButtonText = canUnlockButton.GetComponentInChildren<TextMeshProUGUI>();
            backdropPanel.SetActive(false);   
            canUnlockButton.interactable = false;
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
            skillPointNeedText.text = $"Require {perk.skillPointsToUnlock} skill points";
            skillPointNeedText.gameObject.SetActive(true);
            canUnlockButton.interactable = false;
            canUnlockButtonText.color = Color.grey;
            canUnlockButton.gameObject.SetActive(true);
            
            if (skillIcon.CheckIsUnlocked())
            {
                canUnlockButton.gameObject.SetActive(false);
                skillPointNeedText.gameObject.SetActive(false);
            } 
            else if (!skillIcon.CheckCanUnlock())
            {
                skillPointNeedText.text = "Lock behind previous node";
                skillPointNeedText.color = Color.grey;
            }
            else
            {
                bool canUnlock = PlayerExp.EnoughPoint(perk);
                if (canUnlock)
                {
                    skillPointNeedText.color = Color.green;
                    canUnlockButtonText.color = Color.white;
                }
                else
                {
                    skillPointNeedText.color = Color.red;
                }
                canUnlockButton.interactable = canUnlock;
            }
        }
        
        public void SetWeaponSkillIconInformation(Component component, object skill)
        {
            skillIcon = (SkillIcon)component;
            WeaponUnlockPerk perk = (WeaponUnlockPerk)((object[])skill)[0];
            sidebar.SetActive(true);
            this.titleText.text = perk.Name;
            this.descriptionText.text = perk.Description;
            skillPointNeedText.text = $"Require {perk.skillPointsToUnlock} skill points";
            skillPointNeedText.gameObject.SetActive(true);
            
            canUnlockButton.interactable = false;
            canUnlockButtonText.color = Color.grey;
            canUnlockButton.gameObject.SetActive(true);
            
            if (skillIcon.CheckIsUnlocked())
            {
                canUnlockButton.gameObject.SetActive(false);
                skillPointNeedText.gameObject.SetActive(false);
            }
            else if (!skillIcon.CheckCanUnlock())
            {
                skillPointNeedText.text = "Lock behind previous node";
                skillPointNeedText.color = Color.grey;
            }
            else
            {
                bool canUnlock = PlayerExp.EnoughPoint(perk);
                if (canUnlock)
                {
                    skillPointNeedText.color = Color.green;
                    canUnlockButtonText.color = Color.white;
                }
                else
                {
                    skillPointNeedText.color = Color.red;
                }
                canUnlockButton.interactable = canUnlock;
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
