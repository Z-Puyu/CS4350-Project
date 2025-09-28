namespace DataStructuresForUnity.Runtime.GeneralUtils {
    public interface IActivatable {
        public bool IsActive { get; }
        
        public void Activate();
        
        public void Deactivate();
    }
}
