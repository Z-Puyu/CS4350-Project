using System.Collections.Generic;
using LeanTween.Testing.Scripts;
using UnityEngine;

namespace LeanTween.Testing {
    public class PerformanceTests : MonoBehaviour {

        public bool debug = false;

        public GameObject bulletPrefab;

        private LeanPool bulletPool = new LeanPool();

        private Dictionary<GameObject, int> animIds = new Dictionary<GameObject, int>();

        public float shipSpeed = 1f;
        private float shipDirectionX = 1f;

        // Use this for initialization
        void Start () {

            GameObject[] pool = this.bulletPool.init(this.bulletPrefab, 400, null, true);
            for (int i = 0; i < pool.Length; i++){
                this.animIds[pool[i]] = -1;
            }
        }
	
        // Update is called once per frame
        void Update () {

            // Spray bullets
            for (int i = 0; i < 10; i++)
            {
                GameObject go = this.bulletPool.retrieve();
                int animId = this.animIds[go];
                if (animId >= 0){
                    if (this.debug)
                        Debug.Log("canceling id:" + animId);

                    Framework.LeanTween.cancel(animId);
                }
                go.transform.position = this.transform.position;

                float incr = (float)(5-i) * 0.1f;
                Vector3 to = new Vector3(Mathf.Sin(incr) * 180f, 0f, Mathf.Cos(incr) * 180f);

                this.animIds[go] = Framework.LeanTween.move(go, go.transform.position+to, 5f).setOnComplete(() => {
                    this.bulletPool.giveup(go);
                }).id;
            }

            // Move Ship
            if(this.transform.position.x<-20f){
                this.shipDirectionX = 1f;
            }else if (this.transform.position.x > 20f){
                this.shipDirectionX = -1f;
            }

            var pos = this.transform.position;
            pos.x += this.shipDirectionX * Time.deltaTime * this.shipSpeed;
            this.transform.position = pos;
        }
    }
}
