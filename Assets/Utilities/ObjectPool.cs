using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities {
    [Serializable]
    public class ObjectPool<T> where T : Component {
        [field: SerializeField] public T Prefab { get; private set; }
        private Queue<T> pool = new Queue<T>();

        public void Initialize(int size) {
            this.pool = new Queue<T>(size);
            for (int i = 0; i < size; i++) {
                T instance = Object.Instantiate(this.Prefab);
                instance.gameObject.SetActive(false);
                this.pool.Enqueue(instance);
            }
        }
        
        public T GetInstance(Vector3 position = default, Quaternion rotation = default) {
            if (this.pool.Count == 0) {
                T instance = Object.Instantiate(this.Prefab);
                instance.gameObject.SetActive(false);
                this.pool.Enqueue(instance);
            }
            
            T obj = this.pool.Dequeue();
            obj.gameObject.SetActive(true);
            Transform transform = obj.transform;
            transform.position = position;
            transform.rotation = rotation;
            return obj;
        }

        public void ReturnInstance(T instance) {
            instance.gameObject.SetActive(false);
            this.pool.Enqueue(instance);
        }

        public void Clear() {
            foreach (T instance in this.pool) {
                Object.Destroy(instance.gameObject);
            }
            
            this.pool.Clear();
        }
    }
}
