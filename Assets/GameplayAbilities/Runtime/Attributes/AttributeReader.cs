using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [DisallowMultipleComponent]
    public class AttributeReader : MonoBehaviour, IAttributeReader {
        [field: SerializeField, InfoBox("If unset, will fetch from the first parent with an IAttributeReader.")]
        private AttributeSet Root { get; set; }
        
        bool IAttributeReader.IsTopLevel => false;

        private void Awake() {
            if (!this.Root) {
                this.Root = this.transform.parent.GetComponentInParent<AttributeSet>();
            }
    
            if (!this.Root) {
                Debug.LogError("No root attribute reader found.");
            }
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.Root.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public int GetCurrent(string key) {
            return this.Root.GetCurrent(key);
        }
        
        public int GetMax(string key) {
            return this.Root.GetMax(key);
        }
        
        public int GetMin(string key) {
            return this.Root.GetMin(key);
        }
        
        public Attribute GetAttribute(string key) {
            return this.Root.GetAttribute(key);
        }

        public bool Has(int threshold, string key) {
            return this.Root.Has(threshold, key);
        }

        IEnumerable<Modifier> IAttributeReader.GetModifiers(string key) {
            return ((IAttributeReader)this.Root).GetModifiers(key);
        }

        public int Query(string key, int @base) {
            return this.Root.Query(key, @base);
        }
    }
}
