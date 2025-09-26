using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public interface IAbility {
        public void StartAbility(Vector3 instigatorPosition, Vector3 targetPosition);
        
        /// <summary>
        /// Generate a list of run-time gameplay effects based on the provided execution arguments.
        /// </summary>
        /// <param name="args">The arguments used to invoke the gameplay effects.</param>
        /// <returns>A list of gameplay effects.</returns>
        public IEnumerable<GameplayEffect> GenerateEffects(GameplayEffectExecutionArgs args);

        /// <summary>
        /// Checks if the ability can be used by the instigator.
        /// </summary>
        /// <param name="instigator">The actor who will use the ability.</param>
        /// <param name="target">The target of the ability.</param>
        /// <returns><c>true</c> if the ability can be used, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// This is used to check for more essential conditions, like whether a target type is immune to an ability,
        /// or whether the ability is in cool down. It should not be used to check for costs or probabilistic conditions.
        /// </remarks>
        public bool IsUsable(AttributeSet instigator, AttributeSet target);
        
        public abstract AbilityInfo Info { get; }
    }
}
