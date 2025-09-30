using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Weapons.Runtime {
    public readonly struct AttackAction {
        public IReadOnlyList<string> AttackableTags { get; }
        public LayerMask AttackableLayers { get; }
        public int ComboIndex { get; }
        public Vector3 AttackPoint { get; }
        public Vector3 Direction { get; }

        public AttackAction(
            List<string> attackableTags, LayerMask attackableLayers, Vector3 attackPoint, Vector3 direction,
            int comboIndex = 0
        ) {
            this.AttackableTags = attackableTags;
            this.AttackableLayers = attackableLayers;
            this.AttackPoint = attackPoint;
            this.ComboIndex = comboIndex;
            this.Direction = direction;
        }
    }
}
