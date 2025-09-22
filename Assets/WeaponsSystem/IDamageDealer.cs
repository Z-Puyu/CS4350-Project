using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponsSystem {
    public interface IDamageDealer {
        /// <summary>
        /// Starts an attack.
        /// </summary>
        /// <returns>The combo stage counter.</returns>
        public int StartAttack();

        /// <summary>
        /// Checks for valid targets and deals damage to them.
        /// </summary>
        /// <param name="tags">If non-empty, the target must have one of the tags specified.</param>
        /// <param name="mask">The layer on which the targets are present.
        /// You can ignore this if you use trigger colliders.</param>
        /// <param name="forward">The forward direction the damage dealer is facing.</param>
        public void DealDamage(ICollection<string> tags, LayerMask mask, Vector3 forward);
        
        public void EndAttack();

        public bool AllowsDamageOn(GameObject candidate);
        
        public void ConnectComboResetEvent(Action action);
        
        
    }
}
