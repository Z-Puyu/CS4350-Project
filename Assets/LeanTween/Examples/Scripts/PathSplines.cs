using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
	public class PathSplines : MonoBehaviour {

		public Transform[] trans;
	
		LTSpline cr;
		private GameObject avatar1;

		void OnEnable(){
			// create the path
			this.cr = new LTSpline( new Vector3[] {this.trans[0].position, this.trans[1].position, this.trans[2].position, this.trans[3].position, this.trans[4].position} );
			// cr = new LTSpline( new Vector3[] {new Vector3(-1f,0f,0f), new Vector3(0f,0f,0f), new Vector3(4f,0f,0f), new Vector3(20f,0f,0f), new Vector3(30f,0f,0f)} );
		}

		void Start () {
			this.avatar1 = GameObject.Find("Avatar1");

			// Tween automatically
			Framework.LeanTween.move(this.avatar1, this.cr, 6.5f).setOrientToPath(true).setRepeat(1).setOnComplete( ()=>{
				Vector3[] next = new Vector3[] {this.trans[4].position, this.trans[3].position, this.trans[2].position, this.trans[1].position, this.trans[0].position};
				Framework.LeanTween.moveSpline( this.avatar1, next, 6.5f); // move it back to the start without an LTSpline
			}).setEase(LeanTweenType.easeOutQuad);
		}
	
		private float iter;
		void Update () {
			// Or Update Manually
			// cr.place( avatar1.transform, iter );

			this.iter += Time.deltaTime*0.07f;
			if(this.iter>1.0f)
				this.iter = 0.0f;
		}

		void OnDrawGizmos(){
			// Debug.Log("drwaing");
			if(this.cr==null)
				this.OnEnable();
			Gizmos.color = Color.red;
			if(this.cr!=null)
				this.cr.gizmoDraw(); // To Visualize the path, use this method
		}
	}
}
