using GameplayAbilities.Runtime.Attributes;

namespace WeaponsSystem.DamageHandling {
    public readonly struct Damage {
        public IAttributeReader Instigator { get; }
        public int Magnitude { get; }
    }
}
