using System;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.ObjectPooling;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataStructuresForUnity.Runtime.GeneralUtils {
    public sealed class ObjectSpawner : Singleton<ObjectSpawner> {
        public static GameObject Spawn(GameObject prefab, Transform parent = null) {
            return Object.Instantiate(prefab, parent, true);
        }

        public static GameObject Spawn(
            GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null
        ) {
            return Object.Instantiate(prefab, position, rotation, parent);
        }

        public static T Spawn<T>(T prefab, Transform parent = null) where T : Component {
            return !prefab ? null : Object.Instantiate(prefab, parent, true);
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
                where T : Component {
            return !prefab ? null : Object.Instantiate(prefab, position, rotation, parent);
        }

        public static T Spawn<T>(GameObject prefab, Transform parent = null) where T : Component {
            return ObjectSpawner.Spawn(prefab, parent).GetComponent<T>();
        }

        public static T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
                where T : Component {
            return ObjectSpawner.Spawn(prefab, position, rotation, parent).GetComponent<T>();
        }
    }
}
