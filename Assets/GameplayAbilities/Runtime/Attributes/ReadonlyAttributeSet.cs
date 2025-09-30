using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Trie;
using DataStructuresForUnity.Runtime.Utilities;
using GameplayAbilities.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    public sealed class ReadonlyAttributes : IAttributeReader {
        private IDictionary<string, int> Attributes { get; }

        public ReadonlyAttributes(IEnumerable<KeyValuePair<string, int>> values) {
            this.Attributes = new TrieDictionary<string, char, int>('.');
            foreach (KeyValuePair<string, int> attribute in values) {
                this.Attributes.Add(attribute.Key, attribute.Value);
            }
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.Attributes.Select(selector).GetEnumerator();

            Attribute selector(KeyValuePair<string, int> attribute) {
                return new Attribute(attribute.Key, attribute.Value, attribute.Value, attribute.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        bool IDataReader<string, int>.HasValue(string key, out int value) {
            return this.Attributes.TryGetValue(key, out value);
        }

        IDataReader<string, int> IDataReader<string, int>.With(string key, int value) {
#if DEBUG
            Debug.LogWarning($"{typeof(ReadonlyAttributes)} is read-only. Ignoring {key} = {value}");
#endif
            return this;
        }

        bool IAttributeReader.IsTopLevel => true;

        int IAttributeReader.GetCurrent(string key) {
            return this.Attributes.TryGetValue(key, out int value) ? value : 0;
        }

        int IAttributeReader.GetMax(string key) {
            return this.Attributes.TryGetValue(key, out int value) ? value : 0;
        }

        int IAttributeReader.GetMin(string key) {
            return this.Attributes.TryGetValue(key, out int value) ? value : 0;
        }

        Attribute IAttributeReader.GetAttribute(string key) {
            return new Attribute(key, this.Attributes[key], this.Attributes[key], this.Attributes[key]);
        }

        bool IAttributeReader.Has(int threshold, string key) {
            return this.Attributes.TryGetValue(key, out int value) && value >= threshold;
        }

        IEnumerable<Modifier> IAttributeReader.GetModifiers(string key) {
            return Enumerable.Empty<Modifier>();
        }

        int IAttributeReader.Query(string key, int @base) {
            return this.Attributes.TryGetValue(key, out int value) ? value : @base;
        }
    }
}
