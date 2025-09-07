using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
    public class ObjectPool<T> where T:Component {
        [filed: SerializeField] public T Prefab { get; private set; }
        private Queue<T> pool = new Queue<T>();

        public void Initialize(int size) {
            this.pool = new Queue<T>(size);
            for (int i = 0; i < size; i++) {
                T instance = Object.Instantiate(this.Prefab);
                instance.gameObject.SetActive(false);
                this.pool.Enqueue(instance);
            }
        }
        
        public T GetInstance() {
            if (this.pool.Count == 0) {
                T instance = Object.Instantiate(this.Prefab);
                instance.gameObject.SetActive(false);
                this.pool.Enqueue(instance);
            }
            
            T obj = this.pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void ReturnInstance(T instance) {
            instance.gameObject.SetActive(false);
            this.pool.Enqueue(instance);
        }
    }
}
