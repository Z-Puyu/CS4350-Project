using GameplayAbilities.Runtime.Attributes;
using GameplayEffects.Runtime;
using Projectiles.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    public class AttributeBasedPiercingEffect : ProjectileEffect {
        private readonly struct Instance : IRunnableEffect {
            private Projectile Projectile { get; }
            private ProjectileDurabilityController Controller { get; }
            private int Durability { get; }

            public Instance(Projectile projectile, int durability) {
                this.Projectile = projectile;
                this.Durability = durability;
                this.Controller = this.Projectile.GetController<ProjectileDurabilityController>();
            }
            
            public void Start() {
                this.Controller.Durability = this.Durability;
            }
            
            public void Stop() {
                this.Controller.ReleaseControl();
            }
            
            public void Cancel() {
                this.Controller.ReleaseControl();
            }
        }
        
        [field: SerializeField, TreeDropdown(nameof(this.AttributeOptions))] 
        private string DurabilityAttribute { get; set; }

        private AdvancedDropdownList<string> AttributeOptions => this.GetAttributeOptions();
        
        public override IRunnableEffect Apply(IDataReader<string, int> source, Projectile projectile) {
            return new Instance(projectile, source.HasValue(this.DurabilityAttribute, out var durability) ? durability : 1);
        }

        protected override void HandleHit(
            Vector3 position, GameObject obj, Projectile projectile, IDataReader<string, int> sender
        ) { }
    }
}
