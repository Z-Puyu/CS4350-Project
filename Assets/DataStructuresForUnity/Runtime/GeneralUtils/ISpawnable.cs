namespace DataStructuresForUnity.Runtime.GeneralUtils {
    public interface ISpawnable<T> {
        public void Activate(T args);
        
        public void OnJustActivated(T args);

        public void OnActive(T args);
        
        public void Destroy();
    }
}
