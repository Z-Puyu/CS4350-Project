using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities; 
using WeaponsSystem.Runtime.Weapons;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Runtime.WeaponComponents {
    public class ComponentDatabase : Singleton<ComponentDatabase> {
        [field: SerializeField, ResourceFolder]
        public string ComponentDataFolder { get; private set; }
        private Dictionary<string, WeaponComponent> Components { get; } = new();

        protected override void Awake() {
            base.Awake();

            // Load all WeaponComponent ScriptableObjects from Resources
            foreach (WeaponComponent component in Resources.LoadAll<WeaponComponent>(this.ComponentDataFolder)) {
                if (Components.ContainsKey(component.name)) {
                    Debug.LogError($"Duplicate weapon component name: {component.name}", this);
                    continue;
                }

                Components.Add(component.name, component);
            }

            Debug.Log($"Loaded {Components.Count} weapon components from {ComponentDataFolder}", this);
        }

        public static bool TryGet(string name, out WeaponComponent component) {
            return Instance.Components.TryGetValue(name, out component);
        }

        public static IEnumerable<WeaponComponent> All => Instance.Components.Values;
    }
}
