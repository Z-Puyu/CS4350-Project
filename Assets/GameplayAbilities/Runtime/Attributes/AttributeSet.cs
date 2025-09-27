using System;
using System.Collections;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Trie;
using GameplayAbilities.Runtime.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Runtime.Attributes {
    /// <summary>
    /// A component that manages a set of attributes or stats.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader {
        private IAttributeReader Parent { get; set; }
        
        [field: SerializeField]
        private AttributeData.ModifierMode ModifierMode { get; set; } = AttributeData.ModifierMode.ByPriority;
        
        [field: SerializeField] private AttributeTable DefaultAttributes { get; set; }
        
        private TrieDictionary<string, char, AttributeData> Attributes { get; } =
            new TrieDictionary<string, char, AttributeData>('.');

        /// <summary>
        /// Invoked when an attribute is first initialised or when it is modified.
        /// </summary>
        public event UnityAction<AttributeChange> OnAttributeChanged;

        private void OnEnable() {
            this.ConnectParent();
        }

        private void OnTransformParentChanged() {
            this.ConnectParent();
        }
        
        private void ConnectParent() {
            Transform parent = this.transform.parent;
            if (!parent) {
                this.Parent = null;
            }

            IAttributeReader parentSet = parent.GetComponentInParent<IAttributeReader>();
            if (parentSet != this.Parent) {
                this.Parent = parentSet;
            }
        }

        /// <summary>
        /// Set the initial base values of all attributes using an <see cref="AttributeTable"/>.
        /// </summary>
        /// <param name="table">The table containing the initial values of all attributes.</param>
        /// <remarks>
        /// This method should be called once, before any other method of this class.
        /// </remarks>
        public void Initialise(IEnumerable<KeyValuePair<AttributeType, int>> table = null) {
            table ??= this.DefaultAttributes;
            if (table == null) {
#if DEBUG
                Debug.LogError("No attribute table provided and no default attributes set", this);
#endif
                return;
            }
            
            foreach (KeyValuePair<AttributeType, int> attribute in table) {
                init(attribute.Key, attribute.Value);
            }

            foreach (KeyValuePair<string, AttributeData> data in this.Attributes) {
                this.PostAttributeUpdate(data.Key, data.Value);
            }


            return;

            void init(AttributeType attribute, int value) {
                if (!attribute.IsCategory && !this.Attributes.ContainsKey(attribute.Id)) {
                    AttributeData data = AttributeData.From(attribute, value, this, this.ModifierMode);
                    this.Attributes.Add(attribute.Id, data);
                } else {
                    foreach (AttributeType child in attribute.SubTypes) {
                        init(child, value);
                    }
                }
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
            int newValue = data.RecomputeValue();
            if (oldValue != newValue) {
                this.OnAttributeChanged?.Invoke(new AttributeChange(key, oldValue, newValue));
            }
        }

        /// <summary>
        /// Add a modifier to the attribute set.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        /// <remarks>
        /// You cannot "remove" a modifier because modifiers are value types so you just need to add a negated modifier.
        /// </remarks>
        public void AddModifier(Modifier modifier) {
#if DEBUG
            if (!this.Attributes.ContainsKey(modifier.Target)) {
                Debug.LogWarning($"Trying to add modifier to non-existing attribute {modifier.Target}", this);
                return;
            }
#endif
            this.Attributes.ForEachWithPrefix(modifier.Target, update);
            return;

            void update(string attribute, AttributeData data) {
                data.AddModifier(modifier);
                this.PostAttributeUpdate(attribute, data);
            }
        }

        /// <summary>
        /// Remove a modifier to the attribute set. The modifier must already exist.
        /// If multiple instances of the same modifier exist, the last one will be removed.
        /// </summary>
        /// <param name="modifier">The modifier to remove.</param>
        public void RemoveModifier(Modifier modifier) {
#if DEBUG
            if (!this.Attributes.ContainsKey(modifier.Target)) {
                Debug.LogWarning($"Trying to remove modifier from non-existing attribute {modifier.Target}", this);
                return;
            }
#endif
            this.Attributes.ForEachWithPrefix(modifier.Target, update);
            return;
            
            void update(string attribute, AttributeData data) {
                data.RemoveModifier(modifier);
                this.PostAttributeUpdate(attribute, data);
            }
        }

        public int GetCurrent(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return this.Parent != null ? this.Parent.Query(key, data.Value) : data.Value;
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            return 0;
        }

        public int GetMax(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return data.MaxValue;
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            return int.MaxValue;
        }

        public int GetMin(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return data.MinValue;
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            return int.MinValue;
        }

        public Attribute GetAttribute(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeData data)) {
                return new Attribute(key, data.Value);
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            return new Attribute(key, 0);
        }

        public bool Has(int threshold, string key) {
            return this.GetCurrent(key) >= threshold;
        }

        public int Query(string key, int @base) {
            if (!this.Attributes.TryGetValue(key, out AttributeData data)) {
                return this.Parent != null ? this.Parent.Query(key, @base) : @base;
            }

            int value = data.Query(@base);
            return this.Parent != null ? this.Parent.Query(key, value) : value;
        }
    }
}
