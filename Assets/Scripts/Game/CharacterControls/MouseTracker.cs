using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.CharacterControls {
    public class MouseTracker : MonoBehaviour {
        [field: SerializeField] private float offset;
        private Camera mainCamera;

        public void Awake() {
            this.mainCamera = Camera.main;
        }

        public void Update() {
            this.transform.position = this.transform.parent.position;
            this.transform.localScale = new Vector3( Mathf.Sign(this.transform.parent.localScale.x) * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
            
            Vector3 mousePosScreen = Input.mousePosition;
            Vector3 mousePosWorld = this.mainCamera.ScreenToWorldPoint(mousePosScreen);
            mousePosWorld.z = this.transform.position.z;
            Vector3 direction = (mousePosWorld - this.transform.parent.position).normalized;
            
            this.transform.position += direction * this.offset;
            
            this.transform.rotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.right, direction, Vector3.forward));
            
            Vector3 scale = this.transform.localScale;
            if (Math.Abs(this.transform.rotation.z) > Quaternion.AngleAxis(90, Vector3.forward).z) {
                scale.y = Mathf.Abs(scale.y) * -1;
            } else {
                scale.y = Mathf.Abs(scale.y);
            }

            this.transform.localScale = scale;
        }
    }
}
