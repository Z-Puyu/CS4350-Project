using UnityEngine;

namespace DataStructuresForUnity.Runtime.ObjectPooling {
    public interface IPool<T> {
        public T Pull();
        public T Pull(Vector3 position);
        public T Pull(Vector3 position, Quaternion rotation);
        public T Pull(Transform parent);
        public void Push(T instance);
    }
}
