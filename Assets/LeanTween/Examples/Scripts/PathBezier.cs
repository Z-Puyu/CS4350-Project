using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts{

public class PathBezier : MonoBehaviour {

	public Transform[] trans;
	
	LTBezierPath cr;
	private GameObject avatar1;

	void OnEnable(){
		// create the path
		this.cr = new LTBezierPath( new Vector3[] {this.trans[0].position, this.trans[2].position, this.trans[1].position, this.trans[3].position, this.trans[3].position, this.trans[5].position, this.trans[4].position, this.trans[6].position} );
	}

	void Start () {
		this.avatar1 = GameObject.Find("Avatar1");

		// Tween automatically
		LTDescr descr = Framework.LeanTween.move(this.avatar1, this.cr.pts, 6.5f).setOrientToPath(true).setRepeat(-1);
		Debug.Log("length of path 1:"+this.cr.length);
		Debug.Log("length of path 2:"+descr.optional.path.length);
	}
	
	private float iter;
	void Update () {
		// Or Update Manually
		//cr.place2d( sprite1.transform, iter );

		this.iter += Time.deltaTime*0.07f;
		if(this.iter>1.0f)
			this.iter = 0.0f;
	}

	void OnDrawGizmos(){
		// Debug.Log("drwaing");
		if(this.cr!=null)
			this.OnEnable();
		Gizmos.color = Color.red;
		if(this.cr!=null)
			this.cr.gizmoDraw(); // To Visualize the path, use this method
	}
}

}