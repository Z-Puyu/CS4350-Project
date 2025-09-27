using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem.Projectiles {
    public abstract class ProjectileEffect : ScriptableObject {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public bool EndsSilently { get; private set; }
        
        [field: SerializeField, Table(true, true)]
        public List<AttributeEntry> Attributes { get; set; } = new List<AttributeEntry>();
        
        protected AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();

        public abstract void Execute(
            Projectile projectile, LayerMask mask, IEnumerable<string> tags, ProjectileEffectController controller
        );

        protected abstract ICollection<string> RequiredAttributes { get; }
        
        private void OnValidate() {
            this.Attributes.RemoveAll(attribute => !this.RequiredAttributes.Contains(attribute.Id));
            foreach (string id in this.RequiredAttributes) {
                if (this.Attributes.All(attribute => attribute.Id != id)) {
                    this.Attributes.Add(new AttributeEntry(id, 0, true));
                }
            }
        }
    }
}
