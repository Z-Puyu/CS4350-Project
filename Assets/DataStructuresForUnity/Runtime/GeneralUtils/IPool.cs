using UnityEngine;

namespace DataStructuresForUnity.Runtime.GeneralUtils {
    public interface IPool<T> where T : class {
        public void Initialise(T prefab, int size);

        public void Initialise(int size);
        
        public T CreateInstance();

        public T GetInstance(Vector3 position = default, Quaternion rotation = default);

        public void ReturnInstance(T instance);

        public void Clear();
    }
}
