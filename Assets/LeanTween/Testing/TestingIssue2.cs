using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Testing {
	public class TestingIssue2 : MonoBehaviour {
		public RectTransform rect;
		public GameObject go;
		public GameObject go2;

		private LTDescr descr;

		void Start () {
			this.descr = Framework.LeanTween.move(this.go, new Vector3(0f,0,100f), 10f);
			this.descr.passed = 5f; // this should put it at the midway
			this.descr.updateNow();
			this.descr.pause(); // doesn't matter if pause after or before setting descr.passed I think if I set the passed property and paused the next frame it would work

			//		LeanTween.scale(go2, Vector3.one * 4f, 10f).setEasePunch();

			Framework.LeanTween.scaleX (this.go2, (this.go2.transform.localScale * 1.5f).x, 15f).setEase (LeanTweenType.punch);
			Framework.LeanTween.scaleY (this.go2, (this.go2.transform.localScale * 1.5f).y, 15f).setEase (LeanTweenType.punch);
			Framework.LeanTween.scaleZ (this.go2, (this.go2.transform.localScale * 1.5f).z, 15f).setEase (LeanTweenType.punch);
		}
		bool set = false;
		void Update () {
			if (Time.unscaledTime > 5f && !this.set)
			{
				this.set = true;
				this.descr.resume(); // once this execute the object is put at the midway position as setted by passed and the tween continue.
				Debug.Log("resuming");
			}
		}
	}
}
