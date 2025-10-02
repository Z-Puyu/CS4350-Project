using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
	public class ExampleSpline : MonoBehaviour {

		public Transform[] trans;

		LTSpline spline;
		private GameObject ltLogo;
		private GameObject ltLogo2;

		void Start () {
			this.spline = new LTSpline( new Vector3[] {this.trans[0].position, this.trans[1].position, this.trans[2].position, this.trans[3].position, this.trans[4].position} );
			this.ltLogo = GameObject.Find("LeanTweenLogo1");
			this.ltLogo2 = GameObject.Find("LeanTweenLogo2");

			Framework.LeanTween.moveSpline( this.ltLogo2, this.spline.pts, 1f).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong().setOrientToPath(true);

			LTDescr zoomInPath_LT = Framework.LeanTween.moveSpline(this.ltLogo2, new Vector3[]{Vector3.zero, Vector3.zero, new Vector3(1,1,1), new Vector3(2,1,1), new Vector3(2,1,1)}, 1.5f);
			zoomInPath_LT.setUseEstimatedTime(true);
		}
	
		private float iter;
		void Update () {
			// Iterating over path
			this.ltLogo.transform.position = this.spline.point( this.iter /*(Time.time*1000)%1000 * 1.0 / 1000.0 */);

			this.iter += Time.deltaTime*0.1f;
			if(this.iter>1.0f)
				this.iter = 0.0f;
		}

		void OnDrawGizmos(){
			if(this.spline!=null) 
				this.spline.gizmoDraw(); // debug aid to be able to see the path in the scene inspector
		}
	}
}
