using UnityEngine;

namespace Player_related.Player_exp
{
    [CreateAssetMenu(menuName = "Exp object/Combat exp object", fileName = "Combat exp object")]
    public class EnemyExpObject : ExpObject
    {
        public override void AddExp(PlayerExp playerExp)
        {
            playerExp.AddCombatExp(expValue);
        }
    }
}