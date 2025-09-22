using DataStructuresForUnity.Runtime.GeneralUtils;
using UnityEngine;

namespace Common {
    public sealed class ObjectSpawner : Singleton<ObjectSpawner> {
        public static GameObject Spawn(GameObject prefab, Transform parent = null) {
            return Object.Instantiate(prefab, parent, true);
        }

        public static GameObject Spawn(
            GameObject prefab, Vector3 position, Transform parent = null
        ) {
            return Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }

        public static GameObject Spawn(
            GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null
        ) {
            return Object.Instantiate(prefab, position, rotation, parent);
        }

        public static T Spawn<T>(T prefab, Transform parent = null) where T : Component {
            return Object.Instantiate(prefab, parent, true);
        }

        public static T Spawn<T>(T prefab, Vector3 position, Transform parent = null)
                where T : Component {
            return Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }

        public static T Spawn<T>(
            T prefab, Vector3 position, Quaternion rotation, Transform parent = null
        ) where T : Component {
            return Object.Instantiate(prefab, position, rotation, parent);
        }

        public static T Spawn<T>(GameObject prefab, Transform parent = null) where T : Component {
            return ObjectSpawner.Spawn(prefab, parent).GetComponent<T>();
        }

        public static T Spawn<T>(
            GameObject prefab, Vector3 position, Transform parent = null
        ) where T : Component {
            return ObjectSpawner.Spawn(prefab, position, parent).GetComponent<T>();
        }

        public static T Spawn<T>(
            GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null
        ) where T : Component {
            return ObjectSpawner.Spawn(prefab, position, rotation, parent).GetComponent<T>();
        }
    }
}
