using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace GameplayAbilities.Runtime.Targeting {
    public abstract class TargetingStrategy {
        protected AbilityTargeter Targeter { get; set; }
        protected AbilityCaster Caster { get; set; }
        protected Ability Ability { get; set; }
        protected bool IsTargeting { get; set; }
        protected GameObject AbilityCarrier { get; set; }

        public void Start(AbilityCaster caster, Ability ability, AbilityTargeter targeter) {
            this.Ability = ability;
            this.IsTargeting = true;
            this.Targeter = targeter;
            targeter.TargetingStrategy = this;
            this.Caster = caster;
            this.BeginTargeting();
        }
        
        protected virtual void BeginTargeting() { }

        public void Update() {
            if (this.IsTargeting) {
                this.UpdateTargeting();
            }
        }

        protected virtual void UpdateTargeting() { }

        public void Cancel() {
            if (!this.IsTargeting) {
                return;
            }
            
            this.IsTargeting = false;
            this.CancelTargeting();
        }
        
        protected virtual void CancelTargeting() { }

        public void Confirm() {
            this.ConfirmTarget();
            this.Targeter.TargetingStrategy = null;
        }

        protected abstract void ConfirmTarget();

        public void DelegateTo(GameObject carrier) {
            this.AbilityCarrier = carrier;
        }
        
        public abstract void TargetExplicitly(GameObject explicitTarget);
    }
}
