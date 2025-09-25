using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataStructuresForUnity.Runtime.GeneralUtils {
    public sealed class ObjectSpawner : Singleton<ObjectSpawner> {
        [field: SerializeField, SaintsDictionary("Prefab", "Count")]
        private SaintsDictionary<PoolableObject, int> PoolablePrefabs { get; set; } =
            new SaintsDictionary<PoolableObject, int>();

        private Dictionary<Type, Dictionary<string, ObjectPool<PoolableObject>>> Pools { get; } =
            new Dictionary<Type, Dictionary<string, ObjectPool<PoolableObject>>>();

        protected override void Awake() {
            base.Awake();
            foreach (KeyValuePair<PoolableObject, int> prefab in this.PoolablePrefabs) {
                ObjectPool<PoolableObject> pool = new ObjectPool<PoolableObject>();
                pool.Initialise(prefab.Key, prefab.Value);
                Type type = prefab.Key.GetType();
                if (this.Pools.TryGetValue(type, out Dictionary<string, ObjectPool<PoolableObject>> pools)) {
                    pools.Add(prefab.Key.PoolableId, pool);
                } else {
                    pools = new Dictionary<string, ObjectPool<PoolableObject>> { { prefab.Key.PoolableId, pool } };
                    this.Pools.Add(type, pools);
                }
            }
        }

        public static T Pull<T>(string id, T prefab, Transform parent = null) where T : PoolableObject {
            ObjectSpawner spawner = Singleton<ObjectSpawner>.Instance;
            Type type = typeof(T);
            PoolableObject poolable;
            if (spawner.Pools.TryGetValue(type, out Dictionary<string, ObjectPool<PoolableObject>> pools)) {
                if (pools.TryGetValue(id, out ObjectPool<PoolableObject> pool)) {
                    poolable = pool.GetInstance();
                    poolable.transform.SetParent(parent);
                    return (T)poolable;
                }

                if (!prefab) {
                    return null;
                }
                
                pool = new ObjectPool<PoolableObject>();
                pool.Initialise(prefab);
                pools.Add(id, pool);
                poolable = pool.GetInstance();
                poolable.transform.SetParent(parent);
                return (T)poolable;
            }

            if (!prefab) {
                return null;           
            }
            
            pools = new Dictionary<string, ObjectPool<PoolableObject>>();
            ObjectPool<PoolableObject> p = new ObjectPool<PoolableObject>();
            p.Initialise(prefab);
            pools.Add(id, p);
            spawner.Pools.Add(type, pools);
            poolable = p.GetInstance();
            poolable.transform.SetParent(parent);
            return (T)poolable;
        }

        public static T Pull<T>(string id, T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
                where T : PoolableObject {
            T poolable = ObjectSpawner.Pull(id, prefab, parent);
            if (!poolable) {
                return poolable;
            }

            Transform t = poolable.transform;
            t.position = position;
            t.rotation = rotation;
            return poolable;
        }

        public static GameObject Spawn(GameObject prefab, Transform parent = null) {
            return Object.Instantiate(prefab, parent, true);
        }

        public static GameObject Spawn(
            GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null
        ) {
            return Object.Instantiate(prefab, position, rotation, parent);
        }
        
        public static GameObject Spawn(string id, GameObject prefab, Transform parent = null) {
            PoolableObject obj = ObjectSpawner.Pull(id, prefab.GetComponent<PoolableObject>(), parent);
            return !obj ? ObjectSpawner.Spawn(prefab, parent) : obj.gameObject;
        }

        public static GameObject Spawn(
            string id, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null
        ) {
            PoolableObject obj = ObjectSpawner.Pull(
                id, prefab.GetComponent<PoolableObject>(), position, rotation, parent
            );
            
            return !obj ? ObjectSpawner.Spawn(prefab, position, rotation, parent) : obj.gameObject;
        }

        public static T Spawn<T>(T prefab, Transform parent = null) where T : Component {
            return Object.Instantiate(prefab, parent, true);
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
                where T : Component {
            return Object.Instantiate(prefab, position, rotation, parent);
        }

        public static T Spawn<T>(GameObject prefab, Transform parent = null) where T : Component {
            return ObjectSpawner.Spawn(prefab, parent).GetComponent<T>();
        }

        public static T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
                where T : Component {
            return ObjectSpawner.Spawn(prefab, position, rotation, parent).GetComponent<T>();
        }
        
        public static T Spawn<T>(string id, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
                where T : Component {
            return ObjectSpawner.Spawn(prefab, position, rotation, parent).GetComponent<T>();
        }
    }
}
