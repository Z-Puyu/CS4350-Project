namespace GameplayAbilities.Runtime.GameplayEffects {
    public interface IEffect<in T> {
        public double EffectDuration { get; }
        public IRunnableEffect Apply(T target);
    }

    public interface IEffect<in S, in T> {
        public double EffectDuration { get; }
        public IRunnableEffect Apply(S source, T target);
    }

    public interface IRunnableEffect {
        public void Start();
        public void Stop();
        public void Cancel();
    }
}
