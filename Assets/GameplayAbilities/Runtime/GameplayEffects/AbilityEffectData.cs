using System.Collections.Generic;
using GameplayAbilities.Runtime.Attributes;

namespace GameplayAbilities.Runtime.GameplayEffects {
    public class AbilityEffectData : IDataReader<string, int> {
        private IAttributeReader InstigatorAttributes { get; }
        private IDictionary<string, int> CallerSuppliedDataValues { get; } = new Dictionary<string, int>();
        
        public AbilityEffectData(IAttributeReader instigatorAttributes) {
            this.InstigatorAttributes = instigatorAttributes;
        }
        
        public bool HasValue(string key, out int value) {
            int attributeValue = this.InstigatorAttributes.GetCurrent(key);
            return this.CallerSuppliedDataValues.TryGetValue(key, out value);
        }

        public IDataReader<string, int> With(string key, int value) {
            this.CallerSuppliedDataValues[key] = value;
            return this;
        }
    }
}
