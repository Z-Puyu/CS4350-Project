using System.Collections;
using UnityEngine;

namespace LeanTween.Framework {
	public class LeanTester : MonoBehaviour {
		public float timeout = 15f;

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
		public void Start(){
			this.StartCoroutine( this.timeoutCheck() );
		}

		IEnumerator timeoutCheck(){
			float pauseEndTime = Time.realtimeSinceStartup + this.timeout;
			while (Time.realtimeSinceStartup < pauseEndTime)
			{
				yield return 0;
			}
			if(LeanTest.testsFinished==false){
				Debug.Log(LeanTest.formatB("Tests timed out!"));
				LeanTest.overview();
			}
		}
#endif
	}

	public class LeanTest : object {
		public static int expected = 0;
		private static int tests = 0;
		private static int passes = 0;

		public static float timeout = 15f;
		public static bool timeoutStarted = false;
		public static bool testsFinished = false;
	
		public static void debug( string name, bool didPass, string failExplaination = null){
			LeanTest.expect( didPass, name, failExplaination);
		}

		public static void expect( bool didPass, string definition, string failExplaination = null){
			float len = LeanTest.printOutLength(definition);
			int paddingLen = 40-(int)(len*1.05f);
#if UNITY_FLASH
		string padding = padRight(paddingLen);
#else
			string padding = "".PadRight(paddingLen,"_"[0]);
#endif
			string logName = LeanTest.formatB(definition) +" " + padding + " [ "+ (didPass ? LeanTest.formatC("pass","green") : LeanTest.formatC("fail","red")) +" ]";
			if(didPass==false && failExplaination!=null)
				logName += " - " + failExplaination;
			Debug.Log(logName);
			if(didPass)
				LeanTest.passes++;
			LeanTest.tests++;
		
			// Debug.Log("tests:"+tests+" expected:"+expected);
			if(LeanTest.tests==LeanTest.expected && LeanTest.testsFinished==false){
				LeanTest.overview();
			}else if(LeanTest.tests>LeanTest.expected){
				Debug.Log(LeanTest.formatB("Too many tests for a final report!") + " set LeanTest.expected = "+LeanTest.tests);
			}

			if(LeanTest.timeoutStarted==false){
				LeanTest.timeoutStarted = true;
				GameObject tester = new GameObject();
				tester.name = "~LeanTest";
				LeanTester test = tester.AddComponent(typeof(LeanTester)) as LeanTester;
				test.timeout = LeanTest.timeout;
#if !UNITY_EDITOR
			tester.hideFlags = HideFlags.HideAndDontSave;
#endif
			}
		}
	
		public static string padRight(int len){
			string str = "";
			for(int i = 0; i < len; i++){
				str += "_";
			}
			return str;
		}
	
		public static float printOutLength( string str ){
			float len = 0.0f;
			for(int i = 0; i < str.Length; i++){
				if(str[i]=="I"[0]){
					len += 0.5f;
				}else if(str[i]=="J"[0]){
					len += 0.85f;
				}else{
					len += 1.0f;
				}
			}
			return len;
		}
	
		public static string formatBC( string str, string color ){
			return LeanTest.formatC(LeanTest.formatB(str),color);
		}
	
		public static string formatB( string str ){
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		return str;
#else
			return "<b>"+ str + "</b>";
#endif
		}
	
		public static string formatC( string str, string color ){
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		return str;
#else
			return "<color="+color+">"+ str + "</color>";
#endif
		}
	
		public static void overview(){ 
			LeanTest.testsFinished = true;
			int failedCnt = (LeanTest.expected-LeanTest.passes);
			string failedStr = failedCnt > 0 ? LeanTest.formatBC(""+failedCnt,"red") : ""+failedCnt;
			Debug.Log(LeanTest.formatB("Final Report:")+" _____________________ PASSED: "+LeanTest.formatBC(""+LeanTest.passes,"green")+" FAILED: "+failedStr+" ");
		}
	}
}