using UnityEngine;

namespace WeaponsSystem {
    [CreateAssetMenu(fileName = "AttackData", menuName = "Weapons/AttackData", order = 0)]
    public class AttackData : ScriptableObject {
        [field: SerializeField] private float damageModifier;
        [field: SerializeField] private float knockbackModifier;
    }
}
