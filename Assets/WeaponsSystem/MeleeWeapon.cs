using System.Collections.Generic;
using System.Linq;
using Common;
using GameplayAbilities.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using Weapons.Runtime;
using WeaponsSystem.DamageHandling;
using WeaponsSystem.Projectiles;

namespace WeaponsSystem {
    public sealed class MeleeWeapon : Weapon {
        public override float AttackDuration => 0;

        public override int StartAttack() {
            OnScreenDebugger.Log($"MeleeAttackSuccessfully: combo {this.CurrentAttackCounter}");
            return base.StartAttack();
        }

        public override void DealDamage(
            Combatant combatant, ICollection<string> tags, LayerMask mask, Vector3 forward
        ) {
            int range = this.Stats.GetCurrent(this.Stats.MeleeRangeAttribute);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.AttackOrigin.position, range, mask);
            foreach (Collider2D c in colliders) {
                if (tags.Count > 0 && !tags.Any(c.CompareTag)) {
                    continue;
                }

                float angle = Vector2.Angle(forward, c.transform.position - this.AttackOrigin.position);
                if (angle > this.Stats.SweepHalfAngle) {
                    continue;
                }

                if (!c.TryGetComponent(out IDamageable damageable) || !this.AllowsDamageOn(c.gameObject)) {
                    continue;
                }

                this.Hit(c.bounds.center);
                Damage damage = new Damage(this.transform.root.gameObject, combatant, this.Stats.ReadDamageData());
                damageable.HandleDamage(damage);
            }

            if (this.Stats.ProjectileMode != ProjectileSpawner.Mode.None && this.Stats.ProjectileEffects.Count > 0) {
                // TODO: Spawn Projectiles
            }
        }

        public override bool AllowsDamageOn(GameObject candidate) {
            if (!base.AllowsDamageOn(candidate)) {
                return false;
            }
            
            return !candidate.CompareTag(this.tag) && !candidate.CompareTag(this.transform.root.gameObject.tag);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            int range = 0;
            foreach (KeyValuePair<AttributeType, int> data in this.WeaponData.WeaponAttributes) {
                if (data.Key.Id == this.Stats.MeleeRangeAttribute) {
                    range = data.Value;
                }
            }
            
            Gizmos.DrawWireSphere(this.AttackOrigin.position, range);
        }
    }
}
    