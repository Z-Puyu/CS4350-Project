using System.Collections;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Trie;
using GameplayAbilities.Runtime.GameplayEffects;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.Attributes {
    /// <summary>
    /// A component that manages a set of attributes or stats.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader {
        private TrieDictionary<string, char, AttributeData> Attributes { get; } =
            new TrieDictionary<string, char, AttributeData>();

        /// <summary>
        /// Invoked when an attribute is first initialised or when it is modified.
        /// </summary>
        public event UnityAction<AttributeChange> OnAttributeChanged; 

        /// <summary>
        /// Set the initial base values of all attributes using an <see cref="AttributeTable"/>.
        /// </summary>
        /// <param name="table">The table containing the initial values of all attributes.</param>
        /// <remarks>
        /// This method should be called once, before any other method of this class.
        /// </remarks>
        public void Initialise(AttributeTable table) {
            foreach (KeyValuePair<AttributeTypeDefinition, int> attribute in table) {
                this.Attributes.Add(attribute.Key.Id, AttributeData.From(attribute.Key, attribute.Value, this));
                this.OnAttributeChanged?.Invoke(new AttributeChange(attribute.Key.Id, 0, attribute.Value));
            }

            foreach (KeyValuePair<string, AttributeData> data in this.Attributes) {
                this.PostAttributeUpdate(data.Key, data.Value);
            }
        }
        
        public IEnumerator<Attribute> GetEnumerator() {
            foreach (KeyValuePair<string, AttributeData> entry in this.Attributes) {
                yield return new Attribute(entry.Key, entry.Value.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private void PostAttributeUpdate(string key, AttributeData data) {
            int oldValue = data.Value;
            data.Clamp();
            if (oldValue != data.Value) {
                this.OnAttributeChanged?.Invoke(new AttributeChange(key, oldValue, data.Value));
            }
        }

        /// <summary>
        /// Add a modifier to the attribute set.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        /// <remarks>
        /// You cannot "remove" a modifier because modifiers are value types so you just need to add a negated modifier.
        /// </remarks>
        internal void AddModifier(Modifier modifier) {
            this.Attributes.ForEachWithPrefix(modifier.Target, (_, data) => data.AddModifier(modifier));
        }

        public int GetCurrent(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return data.Value;
            }

            Debug.LogWarning($"Trying to access non-existing attribute {key}", this);
            return 0;
        }

        public int GetMax(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return data.MaxValue;
            }

            Debug.LogWarning($"Trying to access non-existing attribute {key}", this);
            return int.MaxValue;
        }

        public int GetMin(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return data.MinValue;
            }

            Debug.LogWarning($"Trying to access non-existing attribute {key}", this);
            return int.MinValue;
        }

        public Attribute GetAttribute(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return new Attribute(key, data.Value);
            }

            Debug.LogWarning($"Trying to access non-existing attribute {key}", this);
            return new Attribute(key, 0);
        }

        public bool Has(int threshold, string key) {
            return this.GetCurrent(key) >= threshold;
        }
    }
}
