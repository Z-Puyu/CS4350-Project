using UnityEngine;

namespace Player_related.Player_exp
{
    public class PlayerExp : MonoBehaviour
    {
        [SerializeField] private int combatExp;
        [SerializeField] private int farmingExp;
        
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
