using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Runtime {
    public interface IWeapon {
        public void StartAttack();

        public void PerformAttack(
            List<string> attackableTags, LayerMask attackableLayers, Vector3 attackPosition, Vector3 attackDirection
        );

        public void EndAttack();
    }
}
