using Events;
using Game.Enemies;
using GameplayAbilities.Runtime.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player_related.Player_exp
{
    public class PlayerExp : MonoBehaviour
    {
        [SerializeField] private int combatExp;
        [SerializeField] private int farmingExp;
        [SerializeField] private int combatExpLevel;
        [SerializeField] private int farmingExpLevel;
        [SerializeField] private int combatSkillPoint;
        [SerializeField] private int farmingSkillPoint;
        [SerializeField] private CrossObjectEventWithDataSO onCombatLevelUp;
        [SerializeField] private CrossObjectEventWithDataSO onFarmingLevelUp;
        private int combatExpNeededToLevelUp;
        private int farmingExpNeededToLevelUp;
        
        //Cache perk result
        private Perk perk;

        void Start()
        {
            combatExpLevel = 1;
            farmingExpLevel = 1;
            CalculateExpToLevelUp();
        }
        
        public void AddCombatExpFromEnemyData(Component component, object eD)
        {
            EnemyData enemyData = (EnemyData)((object[])eD)[0];
            enemyData.EnemyExpObject.AddExp(this);
        }
        
        public void AddFarmingExpFromHarvesting(Component component, object fEO)
        {
            FarmingExpObject farmingExpObject = (FarmingExpObject)((object[])fEO)[0];
            farmingExpObject.AddExp(this);
        }

        private void CalculateExpToLevelUp()
        {
            combatExpNeededToLevelUp = (int) Mathf.Round(Mathf.Pow(combatExpLevel, 1.75f));
            farmingExpNeededToLevelUp = (int) Mathf.Round(Mathf.Pow(farmingExpLevel, 1.75f));
        }
        
        public void AddFarmingExp(int exp)
        {
            farmingExp += exp;
            bool isLevelUp = false;
            int change = 0;
            while (farmingExp >= farmingExpNeededToLevelUp)
            {
                farmingSkillPoint++;
                farmingExp -= farmingExpNeededToLevelUp;
                farmingExpLevel++;
                CalculateExpToLevelUp();
                isLevelUp = true;
                change++;
            }
            if (isLevelUp)
            {
                onFarmingLevelUp.TriggerEvent(this, change);
            }
        }

        public void AddCombatExp(int exp)
        {
            combatExp += exp;
            bool isLevelUp = false;
            int change = 0;
            while (combatExp >= combatExpNeededToLevelUp)
            {
                combatSkillPoint++;
                combatExp -= combatExpNeededToLevelUp;
                combatExpLevel++;
                CalculateExpToLevelUp();
                isLevelUp = true;
                change++;
            }
            if (isLevelUp)
            {
                onCombatLevelUp.TriggerEvent(this, change);
            }
        }

        public void UpdateExp(Slider combatExpBar, Slider farmExpBar, TextMeshProUGUI numberOfCombatSkillPointText, TextMeshProUGUI numberOfFarmingSkillPointText)
        {
            CalculateExpToLevelUp();
            combatExpBar.value = ((float)combatExp/(float)combatExpNeededToLevelUp)*0.5f;
            farmExpBar.value = ((float)farmingExp/(float)farmingExpNeededToLevelUp)*0.5f;
            numberOfCombatSkillPointText.text = combatSkillPoint.ToString();
            numberOfFarmingSkillPointText.text = farmingSkillPoint.ToString();
        }

        public bool EnoughPoint(Perk perk)
        {
            this.perk = perk;
            if (perk.IsCombatPerk())
            {
                return combatSkillPoint >= perk.skillPointsToUnlock;
            }
            return farmingSkillPoint >= perk.skillPointsToUnlock;
        }

        public void MinusPoint()
        {
            if (perk == null) return;
            
            if (perk.IsCombatPerk())
            {
                combatSkillPoint -= perk.skillPointsToUnlock;
                perk = null;
                return;
            }
             farmingSkillPoint -= perk.skillPointsToUnlock;
             perk = null;
        }
    }
}