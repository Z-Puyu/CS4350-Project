using UnityEngine;

namespace LeanTween.Testing {
	public class TestingDescr : MonoBehaviour {

		private int tweenId;

		public GameObject go;

		// start a tween
		public void startTween(){
			this.tweenId = Framework.LeanTween.moveX(this.go, 10f, 1f).id;
			Debug.Log("tweenId:" + this.tweenId);
		}

		// check tween descr
		public void checkTweenDescr(){
			var descr = Framework.LeanTween.descr(this.tweenId);
			Debug.Log("descr:" + descr);
			Debug.Log("isTweening:"+Framework.LeanTween.isTweening(this.tweenId));
		}
	}
}
