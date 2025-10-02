namespace DataStructuresForUnity.Runtime.Utilities {
    public interface IActivatable {
        public bool IsActive { get; }
        
        public void Activate();
        
        public void Deactivate();
    }
}
