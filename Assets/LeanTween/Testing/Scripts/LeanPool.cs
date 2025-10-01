using System.Collections.Generic;
using UnityEngine;

/**
 * A Pooling System for GameObjects
*/

namespace LeanTween.Testing.Scripts
{
    public class LeanPool : object
    {
        private GameObject[] array;

        private Queue<GameObject> oldestItems;

        private int retrieveIndex = -1;

        public GameObject[] init(GameObject prefab, int count, Transform parent = null, bool retrieveOldestItems = true)
        {
            this.array = new GameObject[count];

            if (retrieveOldestItems)
                this.oldestItems = new Queue<GameObject>();

            for (int i = 0; i < this.array.Length; i++)
            {
                GameObject go = GameObject.Instantiate(prefab, parent);
                go.SetActive(false);

                this.array[i] = go;
            }

            return this.array;
        }

        public void init(GameObject[] array, bool retrieveOldestItems = true){
            this.array = array;

            if (retrieveOldestItems)
                this.oldestItems = new Queue<GameObject>();
        }

        public void giveup(GameObject go)
        {
            go.SetActive(false);
            this.oldestItems.Enqueue(go);
        }

        public GameObject retrieve()
        {
            for (int i = 0; i < this.array.Length; i++)
            {
                this.retrieveIndex++;
                if (this.retrieveIndex >= this.array.Length)
                    this.retrieveIndex = 0;

                if (this.array[this.retrieveIndex].activeSelf == false)
                {
                    GameObject returnObj = this.array[this.retrieveIndex];
                    returnObj.SetActive(true);

                    if (this.oldestItems != null)
                    {
                        this.oldestItems.Enqueue(returnObj);
                    }

                    return returnObj;
                }
            }

            if (this.oldestItems != null)
            {
                GameObject go = this.oldestItems.Dequeue();
                this.oldestItems.Enqueue(go);// put at the end of the queue again

                return go;
            }

            return null;
        }
    }

}