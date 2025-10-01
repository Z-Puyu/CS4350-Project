using System.Collections.Generic;
using UnityEngine;

namespace WeaponsSystem.Runtime.Attacks {
    public readonly struct AttackAction {
        public GameObject Instigator { get; }
        public List<string> AttackableTags { get; }
        public LayerMask AttackableLayers { get; }
        public Vector3 AttackPoint { get; }
        public Vector3 Forward { get; }

        public AttackAction(
            GameObject instigator, List<string> attackableTags, LayerMask attackableLayers, Vector3 attackPoint,
            Vector3 forward
        ) {
            this.Instigator = instigator;
            this.AttackableTags = attackableTags;
            this.AttackableLayers = attackableLayers;
            this.AttackPoint = attackPoint;
            this.Forward = forward;
        }
    }
}
