using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponsSystem.Runtime.DamageHandling {
    public sealed class Damage : IEnumerable<KeyValuePair<string, int>> {
        public GameObject Instigator { get; set; }
        private Dictionary<string, int> Data { get; set; } = new Dictionary<string, int>();
        public int Multiplier { get; set; }
        
        public Damage(GameObject instigator) {
            this.Instigator = instigator;
        }

        public void Set(string key, int value) {
            this.Data[key] = value;
        }

        public int Get(string key) {
            return Mathf.RoundToInt(this.Data.GetValueOrDefault(key) * Math.Max(0, 100 + this.Multiplier) / 100.0f);
        }
        
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() {
            return this.Data.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
