using System;

namespace DataStructuresForUnity.Runtime.ObjectPooling {
    public interface IPoolable<out T> {
        public void Initialise(Action<T> onReturn);
        public void Return();
    }
}
