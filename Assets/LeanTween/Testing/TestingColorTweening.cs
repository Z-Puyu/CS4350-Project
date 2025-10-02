using UnityEngine;

namespace LeanTween.Testing {
	public class TestingColorTweening : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
			Framework.LeanTween.value(this.gameObject, Color.red, Color.green, 1f)
			         .setOnUpdate(this.OnTweenUpdate)
			         .setOnUpdateParam(new object[] { "" + 2 });
		}

		private void OnTweenUpdate( Color update, object obj){
			object[] objArr = obj as object[];
			Debug.Log("update:"+update+" obj:"+objArr[0]);
		
		}
	
	}
}
