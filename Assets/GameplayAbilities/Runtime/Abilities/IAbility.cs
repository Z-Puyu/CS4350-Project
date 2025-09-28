using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;
using GameplayAbilities.Runtime.GameplayEffects;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public interface IAbility {
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

        /// <summary>
        /// Activates the ability at a specific position. This will apply any reflexive effects.
        /// </summary>
        /// <param name="instigator">The instigator.</param>
        /// <param name="position">The position to use the ability at.</param>
        public void Activate(AbilitySystem instigator, Vector3 position);

        /// <summary>
        /// Invokes the ability on the target. This will apply any direct effects.
        /// </summary>
        /// <param name="instigator">The instigator.</param>
        /// <param name="target">The target.</param>
        /// <param name="args">The arguments to pass to the ability.</param>
        public void Invoke(AbilitySystem instigator, AbilitySystem target, GameplayEffectExecutionArgs args = null);

        public abstract AbilityInfo Info { get; }
    }
}
