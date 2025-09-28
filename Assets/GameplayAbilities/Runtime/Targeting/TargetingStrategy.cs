using GameplayAbilities.Runtime.GameplayEffects;

namespace GameplayAbilities.Runtime.Targeting {
    public abstract class TargetingStrategy {
        protected AbilityCaster Caster { get; set; }
        protected Ability Ability { get; set; }
        protected bool IsTargeting { get; set; }

        public void Start(AbilityCaster caster, Ability ability, AbilityTargeter targeter) {
            this.Ability = ability;
            this.IsTargeting = true;
            targeter.TargetingStrategy = this;
            this.Caster = caster;
            this.BeginTargeting();
        }
        
        protected abstract void BeginTargeting();

        public void Update() {
            if (this.IsTargeting) {
                this.UpdateTargeting();
            }
        }

        protected abstract void UpdateTargeting();

        public void Cancel() {
            if (!this.IsTargeting) {
                return;
            }
            
            this.IsTargeting = false;
            this.CancelTargeting();
        }
        
        protected abstract void CancelTargeting();

        protected abstract void ConfirmTarget();
    }
}
