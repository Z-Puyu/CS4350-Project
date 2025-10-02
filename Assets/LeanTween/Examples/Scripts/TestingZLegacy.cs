using System;
using System.Collections;
using LeanTween.Framework;
using UnityEngine;

namespace LeanTween.Examples.Scripts {
	public class TestingZLegacy : MonoBehaviour {
		public AnimationCurve customAnimationCurve;
		public Transform pt1;
		public Transform pt2;
		public Transform pt3;
		public Transform pt4;
		public Transform pt5;
	
		public delegate void NextFunc();
		private int exampleIter = 0;
		private string[] exampleFunctions = new string[] { /**/"updateValue3Example", "loopTestClamp", "loopTestPingPong", "moveOnACurveExample", "customTweenExample", "moveExample", "rotateExample", "scaleExample", "updateValueExample", "delayedCallExample", "alphaExample", "moveLocalExample", "rotateAroundExample", "colorExample" };
		public bool useEstimatedTime = true;
		private GameObject ltLogo;
		private TimingType timingType = TimingType.HalfTimeScale;
		private int descrTimeScaleChangeId;
		private Vector3 origin;

		public enum TimingType{
			SteadyNormalTime,
			IgnoreTimeScale,
			HalfTimeScale,
			VariableTimeScale,
			Length
		}

		void Awake(){
			Framework.LeanTween.init(3200); // This line is optional. Here you can specify the maximum number of tweens you will use (the default is 400).  This must be called before any use of LeanTween is made for it to be effective.
		}

		void Start () {
			this.ltLogo = GameObject.Find("LeanTweenLogo");
			Framework.LeanTween.delayedCall(1f, this.cycleThroughExamples);
			this.origin = this.ltLogo.transform.position;

			//		alphaExample();
		}

		void pauseNow(){
			Time.timeScale = 0f;
			Debug.Log("pausing");
		}

		void OnGUI(){
			string label = this.useEstimatedTime ? "useEstimatedTime" : "timeScale:"+Time.timeScale;
			GUI.Label(new Rect(0.03f*Screen.width,0.03f*Screen.height,0.5f*Screen.width,0.3f*Screen.height), label);
		}
	
		void endlessCallback(){
			Debug.Log("endless");
		}

		void cycleThroughExamples(){
			if(this.exampleIter==0){
				int iter = (int)this.timingType + 1;
				if(iter>(int)TimingType.Length)
					iter = 0;
				this.timingType = (TimingType)iter;
				this.useEstimatedTime = this.timingType==TimingType.IgnoreTimeScale;
				Time.timeScale = this.useEstimatedTime ? 0 : 1f; // pause the Time Scale to show the effectiveness of the useEstimatedTime feature (this is very usefull with Pause Screens)
				if(this.timingType==TimingType.HalfTimeScale)
					Time.timeScale = 0.5f;

				if(this.timingType==TimingType.VariableTimeScale){
					this.descrTimeScaleChangeId = Framework.LeanTween.value( this.gameObject, 0.01f, 10.0f, 3f).setOnUpdate( (float val)=>{
						//Debug.Log("timeScale val:"+val);
						Time.timeScale = val;
					}).setEase(LeanTweenType.easeInQuad).setUseEstimatedTime(true).setRepeat(-1).id;
				}else{
					Debug.Log("cancel variable time");
					Framework.LeanTween.cancel( this.descrTimeScaleChangeId );
				}
			}
			this.gameObject.BroadcastMessage( this.exampleFunctions[ this.exampleIter ] );

			// Debug.Log("cycleThroughExamples time:"+Time.time + " useEstimatedTime:"+useEstimatedTime);
			float delayTime = 1.1f;
			Framework.LeanTween.delayedCall( this.gameObject, delayTime, this.cycleThroughExamples).setUseEstimatedTime(this.useEstimatedTime);

			this.exampleIter = this.exampleIter+1>=this.exampleFunctions.Length ? 0 : this.exampleIter + 1;
		}

		public void updateValue3Example(){
			Debug.Log("updateValue3Example Time:"+Time.time);
			Framework.LeanTween.value( this.gameObject, this.updateValue3ExampleCallback, new Vector3(0.0f, 270.0f, 0.0f), new Vector3(30.0f, 270.0f, 180f), 0.5f ).setEase(LeanTweenType.easeInBounce).setRepeat(2).setLoopPingPong().setOnUpdateVector3(this.updateValue3ExampleUpdate).setUseEstimatedTime(this.useEstimatedTime);
		}

		public void updateValue3ExampleUpdate( Vector3 val){
			//Debug.Log("val:"+val+" obj:"+obj);
		}

		public void updateValue3ExampleCallback( Vector3 val ){
			this.ltLogo.transform.eulerAngles = val;
			// Debug.Log("updateValue3ExampleCallback:"+val);
		}

		public void loopTestClamp(){
			Debug.Log("loopTestClamp Time:"+Time.time);
			GameObject cube1 = GameObject.Find("Cube1");
			cube1.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			Framework.LeanTween.scaleZ( cube1, 4.0f, 1.0f).setEase(LeanTweenType.easeOutElastic).setRepeat(7).setLoopClamp().setUseEstimatedTime(this.useEstimatedTime);//
		}

		public void loopTestPingPong(){
			Debug.Log("loopTestPingPong Time:"+Time.time);
			GameObject cube2 = GameObject.Find("Cube2");
			cube2.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			Framework.LeanTween.scaleY( cube2, 4.0f, 1.0f ).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(4).setUseEstimatedTime(this.useEstimatedTime);
			//LeanTween.scaleY( cube2, 4.0f, 1.0f, LeanTween.options().setEaseOutQuad().setRepeat(8).setLoopPingPong().setUseEstimatedTime(useEstimatedTime) );
		}

		public void colorExample(){
			GameObject lChar = GameObject.Find("LCharacter");
			Framework.LeanTween.color( lChar, new Color(1.0f,0.0f,0.0f,0.5f), 0.5f ).setEase(LeanTweenType.easeOutBounce).setRepeat(2).setLoopPingPong().setUseEstimatedTime(this.useEstimatedTime);
		}

		public void moveOnACurveExample(){
			Debug.Log("moveOnACurveExample Time:"+Time.time);

			Vector3[] path = new Vector3[] { this.origin,this.pt1.position,this.pt2.position,this.pt3.position,this.pt3.position,this.pt4.position,this.pt5.position,this.origin};
			Framework.LeanTween.move( this.ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true).setUseEstimatedTime(this.useEstimatedTime);
		}
	
		public void customTweenExample(){
			Debug.Log("customTweenExample starting pos:"+this.ltLogo.transform.position+" origin:"+this.origin);
		
			Framework.LeanTween.moveX( this.ltLogo, -10.0f, 0.5f ).setEase(this.customAnimationCurve).setUseEstimatedTime(this.useEstimatedTime);
			Framework.LeanTween.moveX( this.ltLogo, 0.0f, 0.5f ).setEase(this.customAnimationCurve).setDelay(0.5f).setUseEstimatedTime(this.useEstimatedTime);
		}
	
		public void moveExample(){
			Debug.Log("moveExample");
		
			Framework.LeanTween.move( this.ltLogo, new Vector3(-2f,-1f,0f), 0.5f).setUseEstimatedTime(this.useEstimatedTime);
			Framework.LeanTween.move( this.ltLogo, this.origin, 0.5f).setDelay(0.5f).setUseEstimatedTime(this.useEstimatedTime);
		}
	
		public void rotateExample(){
			Debug.Log("rotateExample");

			Hashtable returnParam = new Hashtable();
			returnParam.Add("yo", 5.0);
		
			Framework.LeanTween.rotate( this.ltLogo, new Vector3(0f,360f,0f), 1f).setEase(LeanTweenType.easeOutQuad).setOnComplete(this.rotateFinished).setOnCompleteParam(returnParam).setOnUpdate(this.rotateOnUpdate).setUseEstimatedTime(this.useEstimatedTime);
		}

		public void rotateOnUpdate( float val ){
			//Debug.Log("rotating val:"+val);
		}

		public void rotateFinished( object hash ){
			Hashtable h = hash as Hashtable;
			Debug.Log("rotateFinished hash:"+h["yo"]);
		}
	
		public void scaleExample(){
			Debug.Log("scaleExample");
		
			Vector3 currentScale = this.ltLogo.transform.localScale;
			Framework.LeanTween.scale( this.ltLogo, new Vector3(currentScale.x+0.2f,currentScale.y+0.2f,currentScale.z+0.2f), 1f ).setEase(LeanTweenType.easeOutBounce).setUseEstimatedTime(this.useEstimatedTime);
		}
	
		public void updateValueExample(){
			Debug.Log("updateValueExample");
			Hashtable pass = new Hashtable();
			pass.Add("message", "hi");
			Framework.LeanTween.value( this.gameObject, (Action<float, object>)this.updateValueExampleCallback, this.ltLogo.transform.eulerAngles.y, 270f, 1f ).setEase(LeanTweenType.easeOutElastic).setOnUpdateParam(pass).setUseEstimatedTime(this.useEstimatedTime);
		}
	
		public void updateValueExampleCallback( float val, object hash ){
			// Hashtable h = hash as Hashtable;
			// Debug.Log("message:"+h["message"]+" val:"+val);
			Vector3 tmp = this.ltLogo.transform.eulerAngles;
			tmp.y = val;
			this.ltLogo.transform.eulerAngles = tmp;
		}
	
		public void delayedCallExample(){
			Debug.Log("delayedCallExample");
		
			Framework.LeanTween.delayedCall(0.5f, this.delayedCallExampleCallback).setUseEstimatedTime(this.useEstimatedTime);
		}
	
		public void delayedCallExampleCallback(){
			Debug.Log("Delayed function was called");
			Vector3 currentScale = this.ltLogo.transform.localScale;

			Framework.LeanTween.scale( this.ltLogo, new Vector3(currentScale.x-0.2f,currentScale.y-0.2f,currentScale.z-0.2f), 0.5f ).setEase(LeanTweenType.easeInOutCirc).setUseEstimatedTime(this.useEstimatedTime);
		}

		public void alphaExample(){
			Debug.Log("alphaExample");
		
			GameObject lChar = GameObject.Find ("LCharacter");
			Framework.LeanTween.alpha( lChar, 0.0f, 0.5f ).setUseEstimatedTime(this.useEstimatedTime);
			Framework.LeanTween.alpha( lChar, 1.0f, 0.5f ).setDelay(0.5f).setUseEstimatedTime(this.useEstimatedTime);
		}

		public void moveLocalExample(){
			Debug.Log("moveLocalExample");
		
			GameObject lChar = GameObject.Find ("LCharacter");
			Vector3 origPos = lChar.transform.localPosition;
			Framework.LeanTween.moveLocal( lChar, new Vector3(0.0f,2.0f,0.0f), 0.5f ).setUseEstimatedTime(this.useEstimatedTime);
			Framework.LeanTween.moveLocal( lChar, origPos, 0.5f ).setDelay(0.5f).setUseEstimatedTime(this.useEstimatedTime);
		}

		public void rotateAroundExample(){
			Debug.Log("rotateAroundExample");
		
			GameObject lChar = GameObject.Find ("LCharacter");
			Framework.LeanTween.rotateAround( lChar, Vector3.up, 360.0f, 1.0f ).setUseEstimatedTime(this.useEstimatedTime);
		}

		public void loopPause(){
			GameObject cube1 = GameObject.Find("Cube1");
			Framework.LeanTween.pause(cube1);
		}

		public void loopResume(){
			GameObject cube1 = GameObject.Find("Cube1");
			Framework.LeanTween.resume(cube1 );
		}

		public void punchTest(){
			Framework.LeanTween.moveX( this.ltLogo, 7.0f, 1.0f ).setEase(LeanTweenType.punch).setUseEstimatedTime(this.useEstimatedTime);
		}
	}
}
