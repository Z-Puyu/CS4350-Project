namespace GameplayAbilities.Runtime.Targeting {
    public class SelfTargeting : TargetingStrategy {
        protected override void BeginTargeting() {
            this.ConfirmTarget();
        }

        protected override void UpdateTargeting() { }
        
        protected override void CancelTargeting() { }

        protected override void ConfirmTarget() {
            this.Caster.Cast(this.Ability);
            this.IsTargeting = false;
            this.Ability = null;
            this.Caster = null;
        }
    }
}
