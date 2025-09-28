using Game.Enemies;
using UnityEngine;

namespace Player_related.Player_exp
{
    public class PlayerExp : MonoBehaviour
    {
        [SerializeField] private int combatExp;
        [SerializeField] private int farmingExp;

        public void AddCombatExpFromEnemyData(Component component, object eD)
        {
            EnemyData enemyData = (EnemyData)((object[])eD)[0];
            enemyData.EnemyExpObject.AddExp(this);
        }
        
        public void AddFarmingExp(int exp)
        {
            farmingExp += exp;
        }

        public void AddCombatExp(int exp)
        {
            combatExp += exp;
        }

        public void GetExpObject()
        {
            
        }
    }
}
