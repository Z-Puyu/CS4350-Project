using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
	public class PathSpline2d : MonoBehaviour {

		public Transform[] cubes;

		public GameObject dude1;
		public GameObject dude2;

		private LTSpline visualizePath;

		void Start () {
			Vector3[] path = new Vector3[] {
				this.cubes[0].position,
				this.cubes[1].position,
				this.cubes[2].position,
				this.cubes[3].position,
				this.cubes[4].position
			};

			this.visualizePath = new LTSpline( path );
			// move
			Framework.LeanTween.moveSpline(this.dude1, path, 10f).setOrientToPath2d(true).setSpeed(2f);

			// move Local
			Framework.LeanTween.moveSplineLocal(this.dude2, path, 10f).setOrientToPath2d(true).setSpeed(2f);
		}

		void OnDrawGizmos(){
			Gizmos.color = Color.red;
			if(this.visualizePath!=null)
				this.visualizePath.gizmoDraw();
		}
	}
}
