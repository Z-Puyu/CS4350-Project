using UnityEngine;

namespace Game.CharacterControls {
    public class MouseTracker : MonoBehaviour {
        [SerializeField] private float offset = 0.5f;
        private Camera mainCamera;
        private Transform parent;

        private void Awake() {
            mainCamera = Camera.main;
            parent = transform.parent;
        }

        private void Update() {
            if (!parent || !mainCamera) return;

            // 1. Mouse world position
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            // 2. Direction from player to mouse
            Vector3 direction = (mouseWorld - parent.position).normalized;

            // 3. Position AttackOrigin at offset in that direction
            transform.position = parent.position + direction * offset;

            // 4. Rotate AttackOrigin so its local +X points toward mouse
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle-90f);
        }
    }
}