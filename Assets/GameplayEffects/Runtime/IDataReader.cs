using System.Collections.Generic;

namespace GameplayEffects.Runtime {
    public interface IDataReader<K, V> {
        public bool HasValue(K key, out V value);
        public IDataReader<K, V> With(K key, V value);

        public IDataReader<K, V> With(IEnumerable<KeyValuePair<K, V>> values) {
            foreach (KeyValuePair<K, V> value in values) {
                this.With(value.Key, value.Value);
            }
            
            return this;
        }
    }
}
