using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayEffects.Runtime;

namespace GameplayAbilities.Runtime.Attributes {
    public class AttributeDataReader : IDataReader<string, int> {
        private IAttributeReader InstigatorAttributes { get; }
        private IDictionary<string, int> CallerSuppliedDataValues { get; } = new Dictionary<string, int>();
        
        public AttributeDataReader(IAttributeReader instigatorAttributes) {
            this.InstigatorAttributes = instigatorAttributes;
        }
        
        public bool HasValue(string key, out int value) {
            value = this.InstigatorAttributes.GetCurrent(key);
            return value != 0 || this.CallerSuppliedDataValues.TryGetValue(key, out value);
        }

        public IDataReader<string, int> With(string key, int value) {
            this.CallerSuppliedDataValues[key] = value;
            return this;
        }
    }
}
