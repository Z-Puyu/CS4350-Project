using UnityEngine;

namespace GameplayAbilities.Runtime.Projectiles {
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class AbilityProjectile2D : AbilityProjectile {
        private void OnTriggerEnter2D(Collider2D other) {
            if (!this.IsValidTarget(other)) {
                return;
            }

            this.Hit(other.gameObject);
        }

        protected override void OnUpdate() {
            float distance = Vector3.Distance(this.transform.position, this.LaunchPoint);
            if (distance >= this.Range) {
                this.Return();
                return;
            }
            
            this.Transform.position += this.Direction * (Time.deltaTime * this.Speed);
            this.Transform.right = this.Direction;
        }
    }
}
