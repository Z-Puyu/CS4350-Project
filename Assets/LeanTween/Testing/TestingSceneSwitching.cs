using LeanTween.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LeanTween.Testing {
	public class TestingSceneSwitching : MonoBehaviour {

		public GameObject cube;

		private static int sceneIter = 0;

		private int tweenCompleteCnt;

		// Use this for initialization
		void Start () {
			LeanTest.expected = 6;
		
			// Start a couple of tweens and make sure they complete
			this.tweenCompleteCnt = 0;

			Framework.LeanTween.scale(this.cube, new Vector3(3f,3f,3f), 0.1f).setDelay(0.1f).setOnComplete( ()=>{
				this.tweenCompleteCnt++;
			});

			Framework.LeanTween.move(this.cube, new Vector3(3f,3f,3f), 0.1f).setOnComplete( ()=>{
				this.tweenCompleteCnt++;
			});

			Framework.LeanTween.delayedCall(this.cube, 0.1f, ()=>{
				this.tweenCompleteCnt++;
			});

			// Schedule a couple of tweens, make sure some only half complete than switch scenes

			Framework.LeanTween.delayedCall(this.cube, 1f, ()=>{
				Framework.LeanTween.scale(this.cube, new Vector3(3f,3f,3f), 1f).setDelay(0.1f).setOnComplete( ()=>{

				});

				Framework.LeanTween.move(this.cube, new Vector3(3f,3f,3f), 1f).setOnComplete( ()=>{

				});
			});

			// Load next scene
			Framework.LeanTween.delayedCall(this.cube, 0.5f, ()=>{
				LeanTest.expect( this.tweenCompleteCnt==3, "Scheduled tweens completed:"+TestingSceneSwitching.sceneIter);
				if(TestingSceneSwitching.sceneIter<5){
					TestingSceneSwitching.sceneIter++;
					SceneManager.LoadScene(0);
				}
			});
		}
	
	
	}
}
