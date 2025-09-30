using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Runtime {
    [Serializable]
    public class Damage : IEnumerable<KeyValuePair<string, int>> {
        private Dictionary<string, int> Data { get; } = new Dictionary<string, int>();
        public int Multiplier { get; set; }

        public void Set(string key, int value) {
            this.Data[key] = value;
        }

        public int Get(string key) {
            return Mathf.RoundToInt(this.Data.GetValueOrDefault(key, 0) * Math.Max(0, this.Multiplier + 100) / 100.0f);
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() {
            return this.Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
