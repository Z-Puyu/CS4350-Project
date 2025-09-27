using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using WeaponsSystem.DamageHandling;

namespace WeaponsSystem {
    [DisallowMultipleComponent]
    public sealed class Weaponry : MonoBehaviour {
        [field: SerializeField, Required] private Combatant Combatant { get; set; }
        private List<IDamageDealer> DamageDealers { get; } = new List<IDamageDealer>();
        private int CurrentActiveIndex { get; set; }
        private HashSet<int> LockedWeapons { get; } = new HashSet<int>();
        
        public int Size => this.DamageDealers.Count;
        
        private void Awake() {
            this.GetComponentsInChildren(this.DamageDealers);
        }

        private void Start() {
            this.Switch(this.CurrentActiveIndex);
        }

        public void Switch(int index) {
            if (index < 0 || index >= this.DamageDealers.Count) {
                Debug.LogError($"Index {index} is out of bounds for damage dealers list.", this);
                return;
            }

            if (this.LockedWeapons.Contains(index) || !this.Combatant.Equip(this.DamageDealers[index])) {
                return;
            }

            this.DamageDealers[this.CurrentActiveIndex].Disable();
            this.CurrentActiveIndex = index;
            this.DamageDealers[this.CurrentActiveIndex].Enable();
        }
        
        public void Lock(int index) {
            this.LockedWeapons.Add(index);
        }

        public void Unlock(int index) {
            this.LockedWeapons.Remove(index);
        }

        public void UnlockAll() {
            this.LockedWeapons.Clear();
        }
    }
}
