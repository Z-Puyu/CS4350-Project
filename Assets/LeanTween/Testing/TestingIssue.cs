using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Testing {
	public class TestingIssue : MonoBehaviour {

		LTDescr lt,ff;
		int id,fid;

		void Start () {
			Framework.LeanTween.init();
		
			this.lt = Framework.LeanTween.move(this.gameObject,100*Vector3.one,2);
			this.id = this.lt.id;
			Framework.LeanTween.pause(this.id);

			this.ff = Framework.LeanTween.move(this.gameObject,Vector3.zero,2);
			this.fid = this.ff.id;
			Framework.LeanTween.pause(this.fid);
		}

		void Update () {
			if(Input.GetKeyDown(KeyCode.A))
			{
				// Debug.Log("id:"+id);
				Framework.LeanTween.resume(this.id);
			}
			if(Input.GetKeyDown(KeyCode.D))
			{
				Framework.LeanTween.resume(this.fid);
			}
		}
	}
}
