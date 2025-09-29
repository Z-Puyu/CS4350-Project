using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.ObjectPooling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataStructuresForUnity.Runtime.GeneralUtils {
    [Serializable, Obsolete]
    public class ObjectPool<T> : IPool<T> where T : PoolableObject {
        [field: SerializeField] public T Prefab { get; private set; }
        private Queue<T> pool = new Queue<T>();
        private int capacity;

        public void Initialise(T prefab, int size = 20) {
            this.Prefab = prefab;
            this.Initialise(size);
        }

        public void Initialise(int size) {
            this.pool = new Queue<T>(size);
            this.capacity = size;
            for (int i = 0; i < size; i += 1) {
                T instance = Object.Instantiate(this.Prefab);
                instance.gameObject.SetActive(false);
                this.pool.Enqueue(instance);
            }
        }

        public T CreateInstance() {
            T instance = Object.Instantiate(this.Prefab);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public T GetInstance(Vector3 position = default, Quaternion rotation = default) {
            if (this.pool.Count == 0) {
                return this.CreateInstance();
            }
            
            T obj = this.pool.Dequeue();
            obj.gameObject.SetActive(true);
            Transform transform = obj.transform;
            transform.position = position;
            transform.rotation = rotation;
            // obj.OnReturned += () => this.ReturnInstance(obj);
            return obj;
        }

        public void ReturnInstance(T instance) {
            if (this.pool.Count >= this.capacity) {
                Object.Destroy(instance.gameObject);
                return;           
            }
            
            instance.transform.SetParent(null);
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
