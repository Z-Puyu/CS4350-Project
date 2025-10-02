namespace GameplayEffects.Runtime {
    public interface IRunnableEffect {
        public void Start();
        public void Stop();
        public void Cancel();
    }
}
