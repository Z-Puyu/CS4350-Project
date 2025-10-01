using UnityEngine;

namespace LeanTween.Testing {
	public class TestingRotate : MonoBehaviour {

		//method 1 leantween
		public GameObject sun ;
		public GameObject earth;

		//method 2 leantween
		public GameObject sun2;
		public GameObject earth2;

		//method 3 unity3d
		public GameObject sun3;
		public GameObject earth3;

		void Start () {

			//method 1 leantween
			Vector3 sunLocalForEarth = this.earth.transform.InverseTransformPoint(this.sun.transform.position);
			Debug.Log("sunLocalForEarth:"+sunLocalForEarth);
			Framework.LeanTween.rotateAround(this.earth, this.earth.transform.up, 360f, 5.0f).setPoint(sunLocalForEarth).setRepeat(-1);

			//method 2 leantween
			Vector3 sunLocalForEarth2 = this.earth2.transform.InverseTransformPoint(this.sun2.transform.position);
			Framework.LeanTween.rotateAroundLocal(this.earth2, this.earth2.transform.up, 360f, 5.0f).setPoint(sunLocalForEarth2);

		}

		void Update() {

			//method 3 unity3d
			this.earth3.transform.RotateAround(this.sun3.transform.position, this.sun3.transform.up, 72f * Time.deltaTime);

		}
	}
}
