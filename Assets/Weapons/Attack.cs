using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.Modifiers;
using GameplayEffects.Runtime;
using SaintsField;
using UnityEngine;
using WeaponsSystem.Projectiles;
using ProjectileEffect = WeaponsSystem.Projectiles.ProjectileEffect;

namespace Weapons {
    [Serializable]
    public abstract class Attack : IEffect<WeaponStats> {
        public double EffectDuration => 0;

        public abstract IRunnableEffect Apply(WeaponStats target);
    }
}
