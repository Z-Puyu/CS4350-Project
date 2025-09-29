using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructuresForUnity.Runtime.ObjectPooling {
    public static class ObjectPools<T> where T : Component, IPoolable<T> {
        private static Dictionary<T, ObjectPool<T>> Pools { get; } = new Dictionary<T, ObjectPool<T>>();
        
        public static ObjectPool<T> GetPool(T prefab, int size = 100) {
            if (ObjectPools<T>.Pools.TryGetValue(prefab, out ObjectPool<T> pool)) {
                return pool;
            }
            
            pool = new ObjectPool<T>(prefab, size);
            ObjectPools<T>.Pools.Add(prefab, pool);
            return pool;
        }

        public static T Get(T prefab) {
            return ObjectPools<T>.GetPool(prefab).Pull();
        }
        
        public static T Get(T prefab, Vector3 position, Quaternion rotation) {
            return ObjectPools<T>.GetPool(prefab).Pull(position, rotation);
        }
        
        public static T Get(T prefab, Vector3 position) {
            return ObjectPools<T>.GetPool(prefab).Pull(position);
        }
        
        public static T Get(T prefab, Transform parent) {
            return ObjectPools<T>.GetPool(prefab).Pull(parent);
        }
        
        public static T Get(T prefab, Vector3 position, Quaternion rotation, Transform parent) {
            return ObjectPools<T>.GetPool(prefab).Pull(position, rotation, parent);
        }
        
        public static T Get(T prefab, Vector3 position, Transform parent) {
            return ObjectPools<T>.GetPool(prefab).Pull(position, parent);
        }
    }
}
