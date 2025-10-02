using LeanTween.Framework;
using UnityEngine;

// This project demonstrates how you can use the spline behaviour for a multi-track game (like an endless runner style)

namespace LeanTween.Examples.Scripts {
	public class PathSplineTrack : MonoBehaviour {

		public GameObject car;
		public GameObject carInternal;
		public GameObject trackTrailRenderers;

		public Transform[] trackOnePoints;

		private LTSpline track;
		private int trackIter = 1;
		private float trackPosition; // ratio 0,1 of the avatars position on the track

		void Start () {
			// Make the track from the provided transforms
			this.track = new LTSpline( new Vector3[] {this.trackOnePoints[0].position, this.trackOnePoints[1].position, this.trackOnePoints[2].position, this.trackOnePoints[3].position, this.trackOnePoints[4].position, this.trackOnePoints[5].position, this.trackOnePoints[6].position} );

			// Optional technique to show the trails in game
			Framework.LeanTween.moveSpline( this.trackTrailRenderers, this.track, 2f ).setOrientToPath(true).setRepeat(-1);
		}
	
		void Update () {
			// Switch tracks on keyboard input
			float turn = Input.GetAxis("Horizontal");
			if(Input.anyKeyDown){
				if(turn<0f && this.trackIter>0){
					this.trackIter--;
					this.playSwish();
				}else if(turn>0f && this.trackIter < 2){ // We have three track "rails" so stopping it from going above 3
					this.trackIter++;
					this.playSwish();
				}
				// Move the internal local x of the car to simulate changing tracks
				Framework.LeanTween.moveLocalX(this.carInternal, (this.trackIter-1)*6f, 0.3f).setEase(LeanTweenType.easeOutBack);

			}

			// Update avatar's position on correct track
			this.track.place( this.car.transform, this.trackPosition );

			this.trackPosition += Time.deltaTime * 0.03f;// * Input.GetAxis("Vertical"); // Uncomment to have the forward and backwards controlled by the directional arrows

			if (this.trackPosition < 0f) // We need to keep the ratio between 0-1 so after one we will loop back to the beginning of the track
				this.trackPosition = 1f;
			else if(this.trackPosition>1f)
				this.trackPosition = 0f; 
		}

		// Use this for visualizing what the track looks like in the editor (for a full suite of spline tools check out the LeanTween Editor)
		void OnDrawGizmos(){
			LTSpline.drawGizmo( this.trackOnePoints, Color.red);
		}

		// Make your own LeanAudio sounds at http://leanaudioplay.dentedpixel.com
		void playSwish(){
			AnimationCurve volumeCurve = new AnimationCurve( new Keyframe(0f, 0.005464481f, 1.83897f, 0f), new Keyframe(0.1114856f, 2.281785f, 0f, 0f), new Keyframe(0.2482903f, 2.271654f, 0f, 0f), new Keyframe(0.3f, 0.01670286f, 0f, 0f));
			AnimationCurve frequencyCurve = new AnimationCurve( new Keyframe(0f, 0.00136725f, 0f, 0f), new Keyframe(0.1482391f, 0.005405405f, 0f, 0f), new Keyframe(0.2650336f, 0.002480127f, 0f, 0f));

			AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options().setVibrato( new Vector3[]{ new Vector3(0.2f,0.5f,0f)} ).setWaveNoise().setWaveNoiseScale(1000));

			LeanAudio.play( audioClip ); //a:fvb:8,,.00136725,,,.1482391,.005405405,,,.2650336,.002480127,,,8~8,,.005464481,1.83897,,.1114856,2.281785,,,.2482903,2.271654,,,.3,.01670286,,,8~.2,.5,,~~0~~3,1000,1
		}
	}
}
