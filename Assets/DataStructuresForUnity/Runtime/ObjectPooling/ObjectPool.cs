using System.Collections.Generic;
using UnityEngine;

namespace DataStructuresForUnity.Runtime.ObjectPooling {
    public class ObjectPool<T> : IPool<T> where T : Component, IPoolable<T> {
        private Stack<T> Pool { get; set; } = new Stack<T>();
        private T Prefab { get; set; }
        
        public ObjectPool(T prefab, int size) {
            this.Prefab = prefab;
            this.Prefab.gameObject.SetActive(false);
            this.Spawn(size);
        }

        private void Spawn(int count) {
            for (int i = 0; i < count; i += 1) {
                T instance = Object.Instantiate(this.Prefab);
                this.Pool.Push(instance);
            }
        }

        public T Pull() {
            return this.Pull(Vector3.zero, Quaternion.identity);
        }

        public T Pull(Vector3 position) {
            return this.Pull(position, Quaternion.identity);
        }
        
        public T Pull(Vector3 position, Quaternion rotation) {
            if (!this.Pool.TryPop(out T instance)) {
                instance = Object.Instantiate(this.Prefab);
            }
            
            instance.Initialise(this.Push);
            Transform transform = instance.transform;
            transform.position = position;
            transform.rotation = rotation;
            instance.gameObject.SetActive(true);
            return instance;
        }
        
        public T Pull(Transform parent) {
            if (!this.Pool.TryPop(out T instance)) {
                instance = Object.Instantiate(this.Prefab);
            }
            
            instance.Initialise(this.Push);
            instance.transform.SetParent(parent, false);
            instance.gameObject.SetActive(true);
            return instance;
        }
        
        public T Pull(Vector3 position, Quaternion rotation, Transform parent) {
            if (!this.Pool.TryPop(out T instance)) {
                instance = Object.Instantiate(this.Prefab);
            }
            
            instance.Initialise(this.Push);
            Transform transform = instance.transform;
            transform.position = position;
            transform.rotation = rotation;
            instance.transform.SetParent(parent, true);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public T Pull(Vector3 position, Transform parent) {
            return this.Pull(position, Quaternion.identity, parent);
        }

        public void Push(T instance) {
            instance.gameObject.SetActive(false);
            this.Pool.Push(instance);
        }
    }
}
