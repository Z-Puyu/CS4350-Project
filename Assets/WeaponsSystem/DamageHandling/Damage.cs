using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameplayAbilities.Runtime.Abilities;
using GameplayAbilities.Runtime.Attributes;
using GameplayEffects.Runtime;
using UnityEngine;

namespace WeaponsSystem.DamageHandling {
    [Serializable]
    public sealed class Damage : IAbility {
        [field: SerializeReference]
        private List<IEffect<IDataReader<string, int>, AttributeSet>> Effects { get; set; } =
            new List<IEffect<IDataReader<string, int>, AttributeSet>>();

        public bool IsFeasible(IAttributeReader instigator, AttributeSet target) {
            return true;
        }

        public void Execute(IAttributeReader instigator, AttributeSet target) {
            this.Effects.ForEach(effect => effect.Apply(instigator, target).Start());
        }
    }
}
