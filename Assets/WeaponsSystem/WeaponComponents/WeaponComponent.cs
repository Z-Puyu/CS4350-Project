using UnityEngine;

namespace WeaponsSystem.WeaponComponents {
    public class WeaponComponent : MonoBehaviour {
        [field : SerializeField] public WeaponComponentData ComponentData { get; private set; }
        [SerializeField] public string skillId;
    }
}
