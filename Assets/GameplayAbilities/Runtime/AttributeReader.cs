using System.Collections;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime {
    [DisallowMultipleComponent]
    public class AttributeReader : MonoBehaviour, IAttributeReader {
        [field: SerializeField, InfoBox("If unset, will fetch from the first parent with an IAttributeReader.")]
        private SaintsInterface<Component, IAttributeReader> Root { get; set; }

        private void Awake() {
            this.Root ??= new SaintsInterface<Component, IAttributeReader>(
                this.GetComponentInParent<IAttributeReader>() as Component);
            if (this.Root == null) {
                Debug.LogError("No root attribute reader found.");
            }
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.Root.I.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        public int GetCurrent(string key) {
            return this.Root.I.GetCurrent(key);
        }
        
        public int GetMax(string key) {
            return this.Root.I.GetMax(key);
        }
        
        public int GetMin(string key) {
            return this.Root.I.GetMin(key);
        }
        
        public Attribute GetAttribute(string key) {
            return this.Root.I.GetAttribute(key);
        }

        public bool Has(int threshold, string key) {
            return this.Root.I.Has(threshold, key);
        }
    }
}
