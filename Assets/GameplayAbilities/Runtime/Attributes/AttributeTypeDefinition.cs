using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.ModificationRules;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [CreateAssetMenu(fileName = "Attribute Type Definition", menuName = "Gameplay Abilities/Attribute Type Definition")]
    public class AttributeTypeDefinition : ScriptableObject {
        [field: SerializeField] private string Name { get; set; }
        [SerializeField] private string displayName;

        [field: SerializeReference]
        public List<IAttributeClampRule> ModificationRules { get; private set; } =
            new List<IAttributeClampRule>();

        [field: SerializeField, ReadOnly] public string Id { get; private set; }
        [field: SerializeField, ReadOnly] private AttributeTypeDefinition Parent { get; set; }
        
        [field: SerializeField, OnValueChanged(nameof(this.OnSubtypesChanged))]
        public List<AttributeTypeDefinition> SubTypes { get; private set; } = new List<AttributeTypeDefinition>();
        
        public string DisplayName => string.IsNullOrWhiteSpace(this.displayName) ? this.Name : this.displayName;
        public bool IsCategory => this.SubTypes.Count > 0;

        public bool Includes(string attribute) {
            return this.Id == attribute || this.SubTypes.Any(def => def.Includes(attribute));
        }

        private void OnSubtypesChanged() {
            foreach (AttributeTypeDefinition def in this.SubTypes) {
                if (def) {
                    def.Parent = this;
                }
            }
        }

        private void OnValidate() {
            LinkedList<string> names = new LinkedList<string>();
            AttributeTypeDefinition curr = this;
            while (curr) {
                names.AddFirst(curr.Name);
                curr = curr.Parent;
            }
            
            this.Id = string.Join(".", names);
        }
    }
}
