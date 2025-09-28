using UnityEngine;

namespace Player_related.Player_exp
{
    [CreateAssetMenu(menuName = "Exp object/Farming exp object", fileName = "Farming exp object")]
    public class FarmingExpObject : ExpObject
    {
        public override void AddExp(PlayerExp playerExp)
        {
            playerExp.AddFarmingExp(expValue);
        }
    }
}