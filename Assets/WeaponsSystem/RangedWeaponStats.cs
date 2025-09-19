using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;

namespace WeaponsSystem {
    public sealed class RangedWeaponStats : WeaponStats {
        [field: SerializeField, Required]
        private AttributeTypeDefinition RangeAttributeType { get; set; }
        
        public string RangeAttribute => this.RangeAttributeType.Id;
        
        [field: SerializeField, Required]
        private AttributeTypeDefinition BulletSpeedAttributeType { get; set; }
        
        public string BulletSpeedAttribute => this.BulletSpeedAttributeType.Id;
        
        [field: SerializeField, Required]
        private AttributeTypeDefinition BulletSpreadAttributeType { get; set; }
        
        public string BulletSpreadAttribute => this.BulletSpreadAttributeType.Id;
        
        [field: SerializeField, Required]
        private AttributeTypeDefinition FireIntervalAttributeType { get; set; }
        
        public string FireIntervalAttribute => this.FireIntervalAttributeType.Id;
        
        [field: SerializeField, Required]
        private AttributeTypeDefinition PierceStrengthAttributeType { get; set; }
        
        public string PierceStrengthAttribute => this.PierceStrengthAttributeType.Id;
    }
}
