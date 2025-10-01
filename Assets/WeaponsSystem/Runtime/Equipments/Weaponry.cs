using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Runtime.Combat;
using WeaponsSystem.Runtime.Weapons;

namespace WeaponsSystem.Runtime.Equipments {
    [DisallowMultipleComponent]
    public sealed class Weaponry : MonoBehaviour {
        [field: SerializeField, Required] private Combatant Combatant { get; set; }
        private List<Weapon> Weapons { get; } = new List<Weapon>();
        private int CurrentActiveIndex { get; set; }
        private HashSet<int> LockedWeapons { get; } = new HashSet<int>();
        
        public int Size => this.Weapons.Count;
        
        private void Awake() {
            this.GetComponentsInChildren(this.Weapons);
        }

        private void Start() {
            this.Switch(this.CurrentActiveIndex);
        }

        public void Switch(int index) {
            if (index < 0 || index >= this.Weapons.Count) {
                Debug.LogError($"Index {index} is out of bounds for damage dealers list.", this);
                return;
            }

            if (this.LockedWeapons.Contains(index) || !this.Combatant.Equip(this.Weapons[index])) {
                return;
            }
            
            this.CurrentActiveIndex = index;
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
