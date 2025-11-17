using System;
using UnityEngine;
using WeaponsSystem.Runtime.Combat;

namespace Game.CharacterControls {
    public class MouseTracker : MonoBehaviour {
        [SerializeField] private float offset = 0.5f;
        private Camera mainCamera;
        private Transform parent;
        private Combatant combatant;
        
        private void Awake()
        {
            parent = transform.parent;
            combatant = parent.GetComponent<Combatant>();
        }

        private void Start()
        {
            // Ensure a valid camera reference, even if it appears late
            StartCoroutine(EnsureCamera());
        }

        private System.Collections.IEnumerator EnsureCamera()
        {
            while (Camera.main == null)
                yield return null;

            mainCamera = Camera.main;
        }

        private void LateUpdate() {
            if (!parent || !mainCamera) return;
            
            // Do not override Combatant's manual swing motion
            if (combatant != null && combatant.IsSwinging)
                return;

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

            if (Math.Abs(angle) >= 90f) {
                Vector3 localScale = this.transform.localScale;
                localScale.x = Math.Abs(localScale.x) * -1f;
                this.transform.localScale = localScale;
            }
        }
    }
}