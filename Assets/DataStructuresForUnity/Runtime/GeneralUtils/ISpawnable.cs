namespace DataStructuresForUnity.Runtime.GeneralUtils {
    public interface ISpawnable<T> {
        public void Activate(T args);
        
        public void Destroy();
    }
}
