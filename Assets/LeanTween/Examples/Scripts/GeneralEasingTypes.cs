using System.Reflection;
using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
	public class GeneralEasingTypes : MonoBehaviour {

		public float lineDrawScale = 10f;
		public AnimationCurve animationCurve;

		private string[] easeTypes = new string[]{
			"EaseLinear","EaseAnimationCurve","EaseSpring",
			"EaseInQuad","EaseOutQuad","EaseInOutQuad",
			"EaseInCubic","EaseOutCubic","EaseInOutCubic",
			"EaseInQuart","EaseOutQuart","EaseInOutQuart",
			"EaseInQuint","EaseOutQuint","EaseInOutQuint",
			"EaseInSine","EaseOutSine","EaseInOutSine",
			"EaseInExpo","EaseOutExpo","EaseInOutExpo",
			"EaseInCirc","EaseOutCirc","EaseInOutCirc",
			"EaseInBounce","EaseOutBounce","EaseInOutBounce",
			"EaseInBack","EaseOutBack","EaseInOutBack",
			"EaseInElastic","EaseOutElastic","EaseInOutElastic",
			"EasePunch","EaseShake",
		};

		void Start () {

			this.demoEaseTypes();
		}

		private void demoEaseTypes(){
			for(int i = 0; i < this.easeTypes.Length; i++){
				string easeName = this.easeTypes[i];
				Transform obj1 = GameObject.Find(easeName).transform.Find("Line");
				float obj1val = 0f;
				LTDescr lt = Framework.LeanTween.value( obj1.gameObject, 0f, 1f, 5f).setOnUpdate( (float val)=>{
					Vector3 vec = obj1.localPosition;
					vec.x = obj1val*this.lineDrawScale;
					vec.y = val*this.lineDrawScale;

					obj1.localPosition = vec;

					obj1val += Time.deltaTime/5f;
					if(obj1val>1f)
						obj1val = 0f;
				});
				if(easeName.IndexOf("AnimationCurve")>=0){
					lt.setEase(this.animationCurve);
				}else{
					MethodInfo theMethod = lt.GetType().GetMethod("set"+easeName);
					theMethod.Invoke(lt, null);
				}

				if (easeName.IndexOf("EasePunch") >= 0) {
					lt.setScale(1f);
				} else if (easeName.IndexOf("EaseOutBounce") >= 0) {
					lt.setOvershoot(2f);
				}
			}

			Framework.LeanTween.delayedCall(this.gameObject, 10f, this.resetLines);
			Framework.LeanTween.delayedCall(this.gameObject, 10.1f, this.demoEaseTypes);
		}

		private void resetLines(){
			for(int i = 0; i < this.easeTypes.Length; i++){
				Transform obj1 = GameObject.Find(this.easeTypes[i]).transform.Find("Line");
				obj1.localPosition = new Vector3(0f,0f,0f);
			}
		}

	}
}
