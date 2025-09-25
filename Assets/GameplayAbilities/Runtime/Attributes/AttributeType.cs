using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.ModificationRules;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [CreateAssetMenu(fileName = "New Attribute Type", menuName = "Gameplay Abilities/Attribute Type")]
    public class AttributeType : ScriptableObject, IComparable<AttributeType>, IEquatable<AttributeType> {
        [field: SerializeField] private string Name { get; set; }
        [SerializeField] private string displayName;

        [field: SerializeReference, ReferencePicker, HideIf(nameof(this.IsCategory))]
        public List<IAttributeClampRule> ModificationRules { get; private set; } =
            new List<IAttributeClampRule>();

        [field: SerializeField, ReadOnly] public string Id { get; private set; }
        [field: SerializeField, ReadOnly] private AttributeType Parent { get; set; }
        
        [field: SerializeField]
        public List<AttributeType> SubTypes { get; private set; } = new List<AttributeType>();
        
        public string DisplayName => string.IsNullOrWhiteSpace(this.displayName) ? this.Name : this.displayName;
        public bool IsCategory => this.SubTypes.Count > 0;
        public bool IsRoot => !this.Parent;

        public bool Includes(string attribute) {
            return this.Id == attribute || this.SubTypes.Any(def => def.Includes(attribute));
        }

        private void OnValidate() {
            if (this.IsCategory) {
                this.ModificationRules.Clear();
            }
            
            this.Rename();
            foreach (AttributeType def in this.SubTypes) {
                if (!def) {
                    continue;
                }
                
                def.Parent = this;
                def.Rename();
            }
        }

        private void Rename() {
            LinkedList<string> names = new LinkedList<string>();
            AttributeType curr = this;
            while (curr) {
                names.AddFirst(curr.Name);
                curr = curr.Parent;
            }
            
            this.Id = string.Join(".", names);
        }

        public int CompareTo(AttributeType other) {
            return other ? string.CompareOrdinal(this.Id, other.Id) : 1;
        }
        
        public bool Equals(AttributeType other) {
            return this.CompareTo(other) == 0;
        }

        public static IEnumerable<AttributeType> GetAll() {
            return Resources.LoadAll<AttributeType>("").OrderBy(a => a.Id);
        }

        public static IEnumerable<AttributeType> GetAllLeaves() {
            return Resources.LoadAll<AttributeType>("").Where(a => !a.IsCategory).OrderBy(a => a.Id);
        }
    }
}
