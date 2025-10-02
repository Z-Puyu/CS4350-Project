using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Trie;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameplayAbilities.Runtime.Attributes {
    /// <summary>
    /// A component that manages a set of attributes or stats.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AttributeSet : MonoBehaviour, IAttributeReader {
        private IAttributeReader Parent { get; set; }
        IAttributeReader IAttributeReader.Parent => this.Parent;
        
        [field: SerializeField] private AttributeTable DefaultAttributes { get; set; }
        [field: SerializeField] private bool IsRoot { get; set; }
        public bool IsTopLevel => this.IsRoot;

        private TrieDictionary<string, char, AttributeSetNode> Attributes { get; } =
            new TrieDictionary<string, char, AttributeSetNode>('.');

        /// <summary>
        /// Invoked when an attribute is first initialised or when it is modified.
        /// </summary>
        public event UnityAction<AttributeChange> OnAttributeChanged;
        
        private void Awake() {
            foreach (AttributeType type in AttributeType.GetAllLeaves()) {
                this.Attributes.Add(type.Id, AttributeSetNode.From(type, this));
            }
        }

        private void OnEnable() {
            this.ConnectParent();
        }

        private void OnTransformParentChanged() {
            this.ConnectParent();
        }
        
        private void ConnectParent() {
            if (this.IsRoot) {
                this.Parent = null;
                return;
            }
            
            Transform parent = this.transform.parent;
            if (!parent) {
                this.Parent = null;
            }

            IAttributeReader parentSet = parent.GetComponentInParent<IAttributeReader>();
            if (parentSet != this.Parent) {
                this.Parent = parentSet;
            }
            
            this.IsRoot = parentSet == null;
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
                Debug.LogWarning("No attribute table provided and no default attributes set", this);
#endif
                return;
            }
            
            foreach (KeyValuePair<AttributeType, int> attribute in table) {
                init(attribute.Key, attribute.Value);
            }

            foreach (KeyValuePair<string, AttributeSetNode> data in this.Attributes) {
                this.PostAttributeUpdate(data.Key, data.Value);
            }
            
            return;
            
            void init(AttributeType attribute, int value) {
                if (!attribute.IsCategory && this.Attributes.ContainsKey(attribute.Id)) {
                    this.Attributes[attribute.Id].BaseValue = value;
                } else {
                    foreach (AttributeType child in attribute.SubTypes) {
                        init(child, value);
                    }
                }
            }
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.Attributes.Keys.Select(key => this.GetAttribute(key)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private void PostAttributeUpdate(string key, AttributeSetNode node) {
            int oldValue = node.Value;
            int newValue = node.RecomputeValue();
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
            this.Attributes.ForEachWithPrefix(modifier.Target, update);
            return;

            void update(string attribute, AttributeSetNode node) {
                node.AddModifier(modifier);
                this.PostAttributeUpdate(attribute, node);
            }
        }

        /// <summary>
        /// Remove a modifier to the attribute set. The modifier must already exist.
        /// If multiple instances of the same modifier exist, the last one will be removed.
        /// </summary>
        /// <param name="modifier">The modifier to remove.</param>
        public void RemoveModifier(Modifier modifier) {
            this.Attributes.ForEachWithPrefix(modifier.Target, update);
            return;
            
            void update(string attribute, AttributeSetNode node) {
                node.RemoveModifier(modifier);
                this.PostAttributeUpdate(attribute, node);
            }
        }

        private AttributeSetNode CollapseNode(string key) {
            AttributeSetNode node = this.Attributes[key].Clone();
            IAttributeReader current = this.Parent;
            while (!current.IsTopLevel) {
                foreach (Modifier modifier in current.GetModifiers(key)) {
                    node.AddModifier(modifier);
                }
                
                current = current.Parent;
            }
            
            return node;
        }

        public int GetCurrent(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeSetNode node)) {
                return this.IsRoot ? node.Value : this.CollapseNode(key).RecomputeValue();
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this);
#endif
            return 0;
        }

        public int GetMax(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeSetNode data)) {
                return data.MaxValue;
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            return int.MaxValue;
        }

        public int GetMin(string key) {
            if (this.Attributes.TryGetValue(key, out AttributeSetNode data)) {
                return data.MinValue;
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            return int.MinValue;
        }

        public Attribute GetAttribute(string key) {
            return new Attribute(key, this.GetCurrent(key), this.GetMin(key), this.GetMax(key));
        }

        public bool Has(int threshold, string key) {
            return this.GetCurrent(key) >= threshold;
        }

        public IEnumerable<Modifier> GetModifiers(string key) {
            return this.Attributes.TryGetValue(key, out AttributeSetNode node)
                    ? node.CurrentModifiers
                    : Enumerable.Empty<Modifier>();
        }

        public int Query(string key, int @base) {
            if (this.Attributes.TryGetValue(key, out AttributeSetNode node)) {
                return this.IsRoot ? node.EvaluateWithBase(@base) : this.CollapseNode(key).EvaluateWithBase(@base);
            }
#if DEBUG
            Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            return @base;
        }

        public void Set(string key, int value) {
            if (this.Attributes.TryGetValue(key, out AttributeSetNode node)) {
                node.BaseValue = value;
                this.PostAttributeUpdate(key, node);
            } else {
#if DEBUG
                Debug.LogWarning($"Trying to access non-existing attribute {key}", this); 
#endif
            }
        }

        bool IDataReader<string, int>.HasValue(string key, out int value) {
            value = this.GetCurrent(key);
            return this.Attributes.ContainsKey(key);
        }
        
        IDataReader<string, int> IDataReader<string, int>.With(string key, int value) {
            this.Set(key, value);
            return this;
        }
    }
}
