using System;
using UnityEngine;

namespace Weapons.Runtime {
    [Serializable]
    public abstract class AttackStrategy {
        public abstract float Execute(AttackContext context);
        
        public abstract bool AllowsDamageOn(GameObject target);
    }
}
