using UnityEngine;

namespace Player_related.Player_exp
{
    public abstract class ExpObject : ScriptableObject
    {
        [SerializeField] protected int expValue;

        public abstract void AddExp(PlayerExp playerExp);
    }
}
