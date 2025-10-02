using System;
using UnityEngine;

namespace Game.CharacterControls {
    public class MouseTracker : MonoBehaviour {
        [field: SerializeField] private float offset;
        private Camera mainCamera;
        private Transform SelfTransform { get; set; }
        private Transform Parent { get; set; }

        public void Awake() {
            this.SelfTransform = this.transform;
            this.Parent = this.SelfTransform.parent;
            this.mainCamera = Camera.main;
        }

        public void Update() {
            Vector3 position = this.SelfTransform.parent.position;
            Vector3 currScale = this.SelfTransform.localScale;
            Vector3 scale = new Vector3(Mathf.Sign(this.Parent.localScale.x) * Mathf.Abs(currScale.x), currScale.y, currScale.z);
            this.SelfTransform.localScale = scale;
            
            Vector3 mousePosScreen = Input.mousePosition;
            Vector3 mousePosWorld = this.mainCamera.ScreenToWorldPoint(mousePosScreen);
            mousePosWorld.z = position.z;
            Vector3 direction = (mousePosWorld - this.Parent.position).normalized;
            
            position += direction * this.offset;
            this.SelfTransform.position = position;
            this.SelfTransform.rotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.right, direction, Vector3.forward));
            
            if (Math.Abs(this.SelfTransform.rotation.z) > Quaternion.AngleAxis(90, Vector3.forward).z) {
                scale.y = Mathf.Abs(scale.y) * -1;
            } else {
                scale.y = Mathf.Abs(scale.y);
            }

            this.SelfTransform.localScale = scale;
        }
    }
}
