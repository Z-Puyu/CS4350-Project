using System;
using UnityEngine;

namespace Game.CharacterControls {
    public class PlayerTracker : MonoBehaviour {
        [field: SerializeField] private float offset;
        private GameObject player;
        private Transform SelfTransform { get; set; }
        private Transform Parent { get; set; }
        public bool shouldFlip { get; set; }

        public void Awake() {
            this.SelfTransform = this.transform;
            this.Parent = this.SelfTransform.parent;
            this.player = GameObject.FindWithTag("Player");
            this.shouldFlip = false;
        }

        public void Update() {
            Vector3 position = this.SelfTransform.parent.position;
            Vector3 currScale = this.SelfTransform.localScale;
            Vector3 scale = new Vector3(
                Mathf.Sign(this.Parent.localScale.x) * Mathf.Abs(currScale.x), currScale.y, currScale.z
            );
            this.SelfTransform.localScale = scale;

            Vector3 playerPos = this.player.transform.position;
            Vector3 direction = (playerPos - this.Parent.position).normalized;

            position += direction * this.offset;
            this.SelfTransform.position = position;
            this.SelfTransform.rotation = Quaternion.Euler(
                0, 0, Vector3.SignedAngle(Vector3.right, direction, Vector3.forward)
            );

            if (Math.Abs(this.SelfTransform.rotation.z) > Quaternion.AngleAxis(90, Vector3.forward).z) {
                scale.y = Mathf.Abs(scale.y) * -1;
                shouldFlip = true;
            } else {
                scale.y = Mathf.Abs(scale.y);
                shouldFlip = false;
            }

            this.SelfTransform.localScale = scale;
        }
    }
}
