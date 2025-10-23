using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructuresForUnity.Runtime.Trie {
    /// <summary>
    /// Represents a generic dictionary-like data structure implemented as a Trie.
    /// Allows efficient storage and retrieval of key-value pairs where keys are sequences of elements.
    /// </summary>
    /// <typeparam name="K">The type of keys in the dictionary. Must implement <see cref="IEnumerable{T}"/>.</typeparam>
    /// <typeparam name="T">The type of elements in the key sequences.</typeparam>
    /// <typeparam name="V">The type of values stored in the dictionary.</typeparam>
    public class TrieDictionary<K, T, V> : ITrie<K, T>, IDictionary<K, V> where K : IEnumerable<T> {
        private sealed class Entry {
            internal IDictionary<T, Entry> Children { get; } = new SortedDictionary<T, Entry>(Comparer<T>.Default);
            internal bool IsEndOfKey { get; private set; }
            internal bool IsLeaf => this.Children.Count == 0;
            internal K Key { get; private set; }
            internal V Value { get; private set; }
            internal int Count { get; private set; }
            
            private bool MarkAsEndOfKey(K key, V value) {
                if (this.IsEndOfKey) {
                    return false;
                }

                this.Count += 1;
                this.IsEndOfKey = true;
                this.Key = key;
                this.Value = value;
                return true;
            }

            internal bool Clear() {
                if (this.IsLeaf && !this.IsEndOfKey) {
                    return false;
                }
                
                this.IsEndOfKey = false;
                this.Children.Clear();
                this.Key = default;
                this.Value = default;
                this.Count = 0;
                return true;
            }
            
            internal bool TryInsert(K key, V value, IEnumerator<T> enumerator) {
                if (!enumerator.MoveNext()) {
                    return this.MarkAsEndOfKey(key, value);
                }
                
                T element = enumerator.Current;
                while (element is null && enumerator.MoveNext()) {
                    element = enumerator.Current;
                }

                if (element is null) {
                    return this.MarkAsEndOfKey(key, value);
                }
                
                if (this.Children.TryGetValue(element, out Entry entry)) {
                    if (!entry.TryInsert(key, value, enumerator)) {
                        return false;
                    }

                    this.Count += 1;
                    return true;
                }

                this.Count += 1;
                entry = new Entry();
                this.Children.Add(element, entry);
                return entry.TryInsert(key, value, enumerator);
            }
            
            internal bool HasKey(IEnumerator<T> enumerator, bool hasSeparator, T separator, out Entry last) {
                last = null;
                if (!enumerator.MoveNext()) {
                    return hasKey(out last);
                }
                
                T element = enumerator.Current;
                while (element is null && enumerator.MoveNext()) {
                    element = enumerator.Current;
                }

                if (element is null) {
                    return hasKey(out last);
                }
                
                return this.Children.TryGetValue(element, out Entry child) &&
                       child.HasKey(enumerator, hasSeparator, separator, out last);
                
                bool hasKey(out Entry entry) {
                    bool isKey = this.IsEndOfKey && (this.Key?.Any() ?? false) &&
                                 (!hasSeparator || !EqualityComparer<T>.Default.Equals(this.Key.Last(), separator));
                    entry = isKey ? this : null;
                    return isKey;
                }
            }

            internal bool HasPrefix(IEnumerator<T> enumerator, bool hasSeparator, T separator, out Entry last) {
                last = null;
                if (!enumerator.MoveNext()) {
                    return isEndOfPrefix(out last);
                }
                
                T element = enumerator.Current;
                while (element is null && enumerator.MoveNext()) {
                    element = enumerator.Current;
                }

                if (element is null) {
                    return isEndOfPrefix(out last);
                }
                
                return this.Children.TryGetValue(element, out Entry child) &&
                       child.HasPrefix(enumerator, hasSeparator, separator, out last);

                bool isEndOfPrefix(out Entry entry) {
                    bool isEnd = !hasSeparator || this.Children.ContainsKey(separator);
                    entry = isEnd ? this : null;
                    return isEnd;
                }
            }
            
            internal bool Remove(IEnumerator<T> enumerator) {
                if (!enumerator.MoveNext()) {
                    return removeSelf();
                }
                
                T element = enumerator.Current;
                while (element is null && enumerator.MoveNext()) {
                    element = enumerator.Current;
                }
                
                if (element is null) {
                    return removeSelf();
                }

                if (!this.Children.TryGetValue(element, out Entry child) || !child.Remove(enumerator)) {
                    return false;
                }

                this.Count -= 1;
                if (child.IsLeaf && !child.IsEndOfKey) {
                    this.Children.Remove(element);
                }
                    
                return true;
                
                bool removeSelf() {
                    if (!this.IsEndOfKey) {
                        return false;
                    }

                    this.IsEndOfKey = false;
                    this.Count -= 1;
                    this.Value = default;
                    return true;
                }
            }

            internal IEnumerable<KeyValuePair<K, V>> AllChildEntries() {
                if (this.IsEndOfKey) {
                    yield return new KeyValuePair<K, V>(this.Key, this.Value);
                }
                
                foreach (Entry child in this.Children.Values) {
                    foreach (KeyValuePair<K, V> pair in child.AllChildEntries()) {
                        yield return pair;
                    }
                }
            }
        }

        private Entry Root { get; } = new Entry();
        private T Separator { get; }
        private bool HasSeparator { get; }

        public V this[K key] {
            get => this.TryGetValue(key, out V value) ? value : throw new KeyNotFoundException();
            set => this.Add(key, value);
        }

        public int Count { get; private set; }
        public bool IsReadOnly => false;
        public ICollection<K> Keys => this.Root.AllChildEntries().Select(kv => kv.Key).ToArray();
        public ICollection<V> Values => this.Root.AllChildEntries().Select(kv => kv.Value).ToArray();

        public TrieDictionary() {
            this.HasSeparator = false;
        }

        /// <summary>
        /// Create a TrieDictionary with a pre-defined separator token.
        /// Elements in prefix sequences of a key are produced by splitting at separators.
        /// </summary>
        /// <typeparam name="K">The type of keys in the dictionary.
        /// Must implement <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="T">The type of elements in the key sequences.</typeparam>
        /// <typeparam name="V">The type of values stored in the dictionary.</typeparam>
        /// <param name="separator">The separator token.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="separator"/> is null.</exception>
        public TrieDictionary(T separator) {
            if (separator is null) {
                throw new ArgumentNullException(nameof(separator));
            }
            
            this.Separator = separator;
            this.HasSeparator = true;
        }

        IEnumerator<K> IEnumerable<K>.GetEnumerator() {
            return this.Root.AllChildEntries().Select(kv => kv.Key).GetEnumerator();
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            return this.Root.AllChildEntries().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds the specified key-value pair to the TrieDictionary.
        /// If the key already exists, its associated value is updated.
        /// </summary>
        /// <param name="item">The pair to be added</param>
        public void Add(KeyValuePair<K, V> item) {
            this.Add(item.Key, item.Value);
        }
        
        public void Add(K item) {
            if (item is null) {
                throw new ArgumentNullException(nameof(item));
            }
            
            this.Add(item, default);
        }

        /// <summary>
        /// Removes all key-value pairs from the TrieDictionary.
        /// </summary>
        public void Clear() {
            this.Root.Clear();
            this.Count = 0;
        }

        public bool Contains(K item) {
            return item is not null && this.ContainsKey(item);
        }

        public void CopyTo(K[] array, int arrayIndex) {
            this.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Determines whether the specified key-value pair exists in the TrieDictionary.
        /// </summary>
        /// <param name="item">The key-value pair to locate in the dictionary.</param>
        /// <returns>True if the specified key-value pair is found; otherwise, false.</returns>
        public bool Contains(KeyValuePair<K, V> item) {
            return this.TryGetValue(item.Key, out V value) && EqualityComparer<V>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            foreach (KeyValuePair<K, V> kv in this) {
                if (arrayIndex >= array.Length) {
                    break;
                }

                array[arrayIndex] = kv;
                arrayIndex += 1;
            }
        }

        /// <summary>
        /// Removes the specified key-value pair from the TrieDictionary.
        /// </summary>
        /// <param name="item">The key-value pair to remove from the TrieDictionary.</param>
        /// <returns>True if the specified key-value pair was successfully removed; otherwise, false.</returns>
        public bool Remove(KeyValuePair<K, V> item) {
            if (this.TryGetValue(item.Key, out V value) && EqualityComparer<V>.Default.Equals(value, item.Value)) {
                return this.Remove(item.Key);
            }

            return false;
        }

        /// <summary>
        /// Adds a key-value pair to the TrieDictionary. If the key already exists, its value will be updated.
        /// </summary>
        /// <param name="key">The key to add to the TrieDictionary.
        /// Must be a sequence type implementing <see cref="IEnumerable{T}"/>.</param>
        /// <param name="value">The value associated with the specified key.</param>
        public void Add(K key, V value) {
            this.Root.TryInsert(key, value, key.GetEnumerator());
        }

        /// <summary>
        /// Determines whether the TrieDictionary contains a specified key.
        /// Note that this is different from <see cref="ContainsPrefix"/>.
        /// A proper prefix of an existing key is not considered as present!
        /// </summary>
        /// <param name="key">The key to locate in the TrieDictionary.</param>
        /// <returns>True if the specified key exists in the TrieDictionary; otherwise, false.</returns>
        public bool ContainsKey(K key) {
            return this.Root.HasKey(key.GetEnumerator(), this.HasSeparator, this.Separator, out Entry _);
        }

        /// <summary>
        /// Removes the entry with the specified key from the TrieDictionary.
        /// </summary>
        /// <param name="key">The key of the entry to be removed.</param>
        /// <returns>True if the key was successfully found and removed; otherwise, false.</returns>
        public bool Remove(K key) {
            return this.Root.Remove(key.GetEnumerator());
        }
        
        /// <summary>
        /// Attempts to retrieve the value associated with the specified key in the Trie dictionary.
        /// </summary>
        /// <param name="key">The key whose associated value is to be retrieved.
        /// The key must be a sequence of elements compatible with the Trie structure.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the key is found in the Trie dictionary; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(K key, out V value) {
            if (this.Root.HasPrefix(key.GetEnumerator(), this.HasSeparator, this.Separator, out Entry entry)) {
                value = entry.Value;
                return true;
            }
            
            value = default;
            return false;
        }

        /// <summary>
        /// Determines whether the trie set contains any keys with the given prefix.
        /// Unlike <see cref="ContainsKey"/>, this method checks if the specified prefix matches the beginning
        /// of at least one key in the trie set. 
        /// </summary>
        /// <param name="prefix">The sequence of elements representing the prefix to search for in the trie set.</param>
        /// <returns>True if the prefix exists at the start of any key in the trie set; otherwise, false.</returns>
        /// <remarks>
        /// Only prefixes stored in the trie whose last element is marked to be the end of a token are considered valid.
        /// </remarks>
        /// <example>
        /// If the trie uses <c>'.'</c> for the separator and <c>"Item.Weapon.Sword"</c> is added as a key,
        /// then only prefixes <c>"Item.Weapon.Sword"</c>, <c>"Item.Weapon"</c> and <c>"Item"</c> are considered
        /// present in the trie, whereas <c>"Item.Weap"</c> is not present in the trie.
        /// If the trie does not define a separator, any prefix string of an existing key is considered present.
        /// </example>
        public bool ContainsPrefix(IEnumerable<T> prefix) {
            return prefix is not null && 
                   this.Root.HasPrefix(prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Entry _);
        }

        public IEnumerable<K> PrefixSearch(IEnumerable<T> prefix) {
            if (prefix is null ||
                !this.Root.HasPrefix(prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Entry entry)) {
                return Enumerable.Empty<K>();
            }

            return entry.AllChildEntries().Select(kv => kv.Key);
        }

        public bool RemoveAllWithPrefix(IEnumerable<T> prefix) {
            return prefix is not null && this.Root.HasPrefix(
                prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Entry last
            ) && last.Clear();
        }

        public bool Remove(IEnumerable<T> key) {
            return key is not null && this.Root.Remove(key.GetEnumerator());
        }

        public IEnumerable<K> Enumerate() {
            return this.Root.AllChildEntries().Select(kv => kv.Key);
        }

        public IEnumerable<KeyValuePair<K, V>> PrefixSearchEntry(IEnumerable<T> prefix) {
            return this.Root.HasPrefix(prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Entry e)
                    ? e.AllChildEntries()
                    : Enumerable.Empty<KeyValuePair<K, V>>();
        }

        public S Aggregate<S>(IEnumerable<T> prefix, S seed, Func<S, V, S> aggregator) {
            return this.Root.HasPrefix(prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Entry e) 
                    ? e.AllChildEntries().Aggregate(seed, (current, kv) => aggregator(current, kv.Value))
                    : seed;
        }
        
        public void ForEachWithPrefix(IEnumerable<T> prefix, Action<K, V> action) {
            if (prefix is null ||
                !this.Root.HasPrefix(prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Entry entry)) {
                return;
            }

            foreach (KeyValuePair<K, V> kv in entry.AllChildEntries()) {
                action(kv.Key, kv.Value);
            }
        }
    }
}
