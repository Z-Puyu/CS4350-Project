using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
	public class GeneralSimpleUI : MonoBehaviour {
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5

		public RectTransform button;

		void Start () {
			Debug.Log("For better examples see the 4.6_Examples folder!");
			if(this.button==null){
				Debug.LogError("Button not assigned! Create a new button via Hierarchy->Create->UI->Button. Then assign it to the button variable");
				return;
			}
		
			// Tweening various values in a block callback style
			Framework.LeanTween.value(this.button.gameObject, this.button.anchoredPosition, new Vector2(200f,100f), 1f ).setOnUpdate( 
				(Vector2 val)=>{
					this.button.anchoredPosition = val;
				}
			);

			Framework.LeanTween.value(this.gameObject, 1f, 0.5f, 1f ).setOnUpdate( 
				(float volume)=>{
					Debug.Log("volume:"+volume);
				}
			);

			Framework.LeanTween.value(this.gameObject, this.gameObject.transform.position, this.gameObject.transform.position + new Vector3(0,1f,0), 1f ).setOnUpdate( 
				(Vector3 val)=>{
					this.gameObject.transform.position = val;
				}
			);

			Framework.LeanTween.value(this.gameObject, Color.red, Color.green, 1f ).setOnUpdate( 
				(Color val)=>{
					UnityEngine.UI.Image image = (UnityEngine.UI.Image)this.button.gameObject.GetComponent( typeof(UnityEngine.UI.Image) );
					image.color = val;
				}
			);

			// Tweening Using Unity's new Canvas GUI System
			Framework.LeanTween.move(this.button, new Vector3(200f,-100f,0f), 1f).setDelay(1f);
			Framework.LeanTween.rotateAround(this.button, Vector3.forward, 90f, 1f).setDelay(2f);
			Framework.LeanTween.scale(this.button, this.button.localScale*2f, 1f).setDelay(3f);
			Framework.LeanTween.rotateAround(this.button, Vector3.forward, -90f, 1f).setDelay(4f).setEase(LeanTweenType.easeInOutElastic);
		}

#else
	void Start(){
		Debug.LogError("Unity 4.6+ is required to use the new UI");
	}
	
#endif
	}
}
