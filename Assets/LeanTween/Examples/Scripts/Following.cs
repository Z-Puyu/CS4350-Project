using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
    public class Following : MonoBehaviour {

        public Transform planet;

        public Transform followArrow;

        public Transform dude1;
        public Transform dude2;
        public Transform dude3;
        public Transform dude4;
        public Transform dude5;

        public Transform dude1Title;
        public Transform dude2Title;
        public Transform dude3Title;
        public Transform dude4Title;
        public Transform dude5Title;

        private Color dude1ColorVelocity;

        private Vector3 velocityPos;

        private void Start()
        {
            this.followArrow.gameObject.LeanDelayedCall(3f, moveArrow).setOnStart(this.moveArrow).setRepeat(-1);

            // Follow Local Y Position of Arrow
            Framework.LeanTween.followDamp(this.dude1, this.followArrow, LeanProp.localY, 1.1f);
            Framework.LeanTween.followSpring(this.dude2, this.followArrow, LeanProp.localY, 1.1f);
            Framework.LeanTween.followBounceOut(this.dude3, this.followArrow, LeanProp.localY, 1.1f);
            Framework.LeanTween.followSpring(this.dude4, this.followArrow, LeanProp.localY, 1.1f, -1f, 1.5f, 0.8f);
            Framework.LeanTween.followLinear(this.dude5, this.followArrow, LeanProp.localY, 50f);

            // Follow Arrow color
            Framework.LeanTween.followDamp(this.dude1, this.followArrow, LeanProp.color, 1.1f);
            Framework.LeanTween.followSpring(this.dude2, this.followArrow, LeanProp.color, 1.1f);
            Framework.LeanTween.followBounceOut(this.dude3, this.followArrow, LeanProp.color, 1.1f);
            Framework.LeanTween.followSpring(this.dude4, this.followArrow, LeanProp.color, 1.1f, -1f, 1.5f, 0.8f);
            Framework.LeanTween.followLinear(this.dude5, this.followArrow, LeanProp.color, 0.5f);

            // Follow Arrow scale
            Framework.LeanTween.followDamp(this.dude1, this.followArrow, LeanProp.scale, 1.1f);
            Framework.LeanTween.followSpring(this.dude2, this.followArrow, LeanProp.scale, 1.1f);
            Framework.LeanTween.followBounceOut(this.dude3, this.followArrow, LeanProp.scale, 1.1f);
            Framework.LeanTween.followSpring(this.dude4, this.followArrow, LeanProp.scale, 1.1f, -1f, 1.5f, 0.8f);
            Framework.LeanTween.followLinear(this.dude5, this.followArrow, LeanProp.scale, 5f);

            // Titles
            var titleOffset = new Vector3(0.0f, -20f, -18f);
            Framework.LeanTween.followDamp(this.dude1Title, this.dude1, LeanProp.localPosition, 0.6f).setOffset(titleOffset);
            Framework.LeanTween.followSpring(this.dude2Title, this.dude2, LeanProp.localPosition, 0.6f).setOffset(titleOffset);
            Framework.LeanTween.followBounceOut(this.dude3Title, this.dude3, LeanProp.localPosition, 0.6f).setOffset(titleOffset);
            Framework.LeanTween.followSpring(this.dude4Title, this.dude4, LeanProp.localPosition, 0.6f, -1f, 1.5f, 0.8f).setOffset(titleOffset);
            Framework.LeanTween.followLinear(this.dude5Title, this.dude5, LeanProp.localPosition, 30f).setOffset(titleOffset);

            // Rotate Planet
            var localPos = Camera.main.transform.InverseTransformPoint(this.planet.transform.position);
            Framework.LeanTween.rotateAround(Camera.main.gameObject, Vector3.left, 360f, 300f).setPoint(localPos).setRepeat(-1);
        }

        private float fromY;
        private float velocityY;
        private Vector3 fromVec3;
        private Vector3 velocityVec3;
        private Color fromColor;
        private Color velocityColor;

        private void Update()
        {
            // Use the smooth methods to follow variables in which ever manner you wish!
            this.fromY = LeanSmooth.spring(this.fromY, this.followArrow.localPosition.y, ref this.velocityY, 1.1f);
            this.fromVec3 = LeanSmooth.spring(this.fromVec3, this.dude5Title.localPosition, ref this.velocityVec3, 1.1f);
            this.fromColor = LeanSmooth.spring(this.fromColor, this.dude1.GetComponent<Renderer>().material.color, ref this.velocityColor, 1.1f);
            Debug.Log("Smoothed y:" + this.fromY + " vec3:" + this.fromVec3 + " color:" + this.fromColor);
        }

        private void moveArrow()
        {
            Framework.LeanTween.moveLocalY(this.followArrow.gameObject, Random.Range(-100f, 100f), 0f);

            var randomCol = new Color(Random.value, Random.value, Random.value);
            Framework.LeanTween.color(this.followArrow.gameObject, randomCol, 0f);

            var randomVal = Random.Range(5f, 10f);
            this.followArrow.localScale = Vector3.one * randomVal;
        }
    }
}
