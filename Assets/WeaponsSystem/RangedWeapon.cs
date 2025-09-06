using UnityEngine;
using Utilities;

namespace WeaponsSystem {
    public class RangedWeapon : Weapon{
        [field: SerializeField] public RangedWeaponData WeaponData { get; private set; }
        private ObjectPool<Bullet> bulletPool;
        
        public override void Attack() {
            Debug.Log("Ranged Attack");

            if (this.bulletPool != null) {
                Bullet bullet = this.bulletPool.GetInstance();
                Vector3 mousePosScreen = Input.mousePosition;
                Vector3 direction = Vector3.forward;
                if (Camera.main != null) {
                    Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
                    direction = (mousePosWorld - this.transform.position).normalized;
                }

                bullet.SetTarget(this.WeaponData.BulletSpeed, this.WeaponData.Range, direction, this.bulletPool);
            }
        }
    }
}
