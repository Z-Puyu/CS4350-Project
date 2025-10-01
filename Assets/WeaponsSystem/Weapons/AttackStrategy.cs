using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponsSystem.Weapons {
    [Serializable]
    public abstract class AttackStrategy {
        public abstract float Execute(AttackContext context);
        
        public abstract bool AllowsDamageOn(GameObject target);
    }
}
