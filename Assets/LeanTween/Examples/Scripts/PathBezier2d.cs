using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
	public class PathBezier2d : MonoBehaviour {

		public Transform[] cubes;

		public GameObject dude1;
		public GameObject dude2;

		private LTBezierPath visualizePath;

		void Start () {
			// move 
			Vector3[] path = new Vector3[]{this.cubes[0].position,this.cubes[1].position,this.cubes[2].position,this.cubes[3].position};
			// 90 degree test
			// path = new Vector3[] {new Vector3(7.5f, 0f, 0f), new Vector3(0f, 0f, 2.5f), new Vector3(2.5f, 0f, 0f), new Vector3(0f, 0f, 7.5f)};
			this.visualizePath = new LTBezierPath(path);
			Framework.LeanTween.move(this.dude1, path, 10f).setOrientToPath2d(true);

			// move local
			Framework.LeanTween.moveLocal(this.dude2, path, 10f).setOrientToPath2d(true);
		}

		void OnDrawGizmos(){
			// Debug.Log("drwaing");
			Gizmos.color = Color.red;
			if(this.visualizePath!=null)
				this.visualizePath.gizmoDraw(); // To Visualize the path, use this method
		}
	}
}
