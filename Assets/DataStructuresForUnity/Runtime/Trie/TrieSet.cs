using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructuresForUnity.Runtime.Trie {
    /// <summary>
    /// A set abstract data type implemented using a trie.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="T">The element type.</typeparam>
    public sealed class TrieSet<K, T> : ITrie<K, T>, ISet<K> where K : IEnumerable<T> {
        private sealed class Node {
            internal int Count { get; private set; }
            private IDictionary<T, Node> Children { get; } = new SortedDictionary<T, Node>(Comparer<T>.Default);
            private bool IsLeaf => this.Children.Count == 0;
            private bool IsEndOfKey { get; set; }
            private K Value { get; set; }

            private bool MarkAsEndOfKey(K key) {
                if (this.IsEndOfKey) {
                    return false;
                }

                this.Count += 1;
                this.IsEndOfKey = true;
                this.Value = key;
                return true;
            }
            
            internal bool TryInsert(K key, IEnumerator<T> enumerator) {
                if (!enumerator.MoveNext()) {
                    return this.MarkAsEndOfKey(key);
                }
                
                T element = enumerator.Current;
                while (element is null && enumerator.MoveNext()) {
                    element = enumerator.Current;
                }

                if (element is null) {
                    return this.MarkAsEndOfKey(key);
                }
                
                if (this.Children.TryGetValue(element, out Node node)) {
                    if (!node.TryInsert(key, enumerator)) {
                        return false;
                    }

                    this.Count += 1;
                    return true;
                }

                this.Count += 1;
                node = new Node();
                this.Children.Add(element, node);
                return node.TryInsert(key, enumerator);
            }

            internal bool HasKey(IEnumerator<T> enumerator, bool hasSeparator, T separator, out Node last) {
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
                
                return this.Children.TryGetValue(element, out Node child) &&
                       child.HasKey(enumerator, hasSeparator, separator, out last);
                
                bool hasKey(out Node node) {
                    bool isKey = this.IsEndOfKey && (this.Value?.Any() ?? false) &&
                                 (!hasSeparator || !EqualityComparer<T>.Default.Equals(this.Value.Last(), separator));
                    node = isKey ? this : null;
                    return isKey;
                }
            }

            internal bool HasPrefix(IEnumerator<T> enumerator, bool hasSeparator, T separator, out Node last) {
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
                
                return this.Children.TryGetValue(element, out Node child) &&
                       child.HasPrefix(enumerator, hasSeparator, separator, out last);

                bool isEndOfPrefix(out Node node) {
                    bool isEnd = !hasSeparator || this.Children.ContainsKey(separator);
                    node = isEnd ? this : null;
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

                if (!this.Children.TryGetValue(element, out Node child) || !child.Remove(enumerator)) {
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

            internal bool Clear() {
                if (this.IsLeaf && !this.IsEndOfKey) {
                    return false;
                }
                
                this.Children.Clear();
                this.Count = 0;
                this.IsEndOfKey = false;
                this.Value = default;
                return true;
            }

            internal IEnumerable<K> AllChildKeys() {
                if (this.IsEndOfKey) {
                    yield return this.Value;
                }
                
                foreach (Node child in this.Children.Values) {
                    foreach (K key in child.AllChildKeys()) {
                        yield return key;
                    }
                }
            }
        }

        private Node Root { get; } = new Node();
        private T Separator { get; }
        private bool HasSeparator { get; }
        public int Count => this.Root.Count;
        public bool IsReadOnly => false;

        public TrieSet() {
            this.HasSeparator = false;
        }

        /// <summary>
        /// Create a TrieSet with a pre-defined separator token.
        /// Elements in prefix sequences of a key are produced by splitting at separators.
        /// </summary>
        /// <typeparam name="K">The type of keys stored in the trie set,
        /// must implement <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="T">The type of elements used to form the keys.</typeparam>
        /// <param name="separator">The token used to split keys into prefixes.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="separator"/> is null.</exception>   
        public TrieSet(T separator) {
            if (separator is null) {
                throw new ArgumentNullException(nameof(separator));   
            }
            
            this.Separator = separator;
            this.HasSeparator = true;
        }

        public IEnumerator<K> GetEnumerator() {
            return this.Root.AllChildKeys().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds a key to the TrieSet. If the key already exists in the set, it is not added again.
        /// </summary>
        /// <param name="key">The key to be added to the set.
        /// Each key is represented as a sequence of tokens of type <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="key"/> is null.</exception>
        public void Add(K key) {
            if (key is null) {
                throw new ArgumentNullException(nameof(key));
            }

            this.Root.TryInsert(key, key.GetEnumerator());
        }

        /// <summary>
        /// Removes all elements in the current set that are also in the specified collection.
        /// </summary>
        /// <param name="other">The collection of elements to remove from the set.
        /// Each element from <paramref name="other"/> will be removed if it exists in the current set.</param>
        public void ExceptWith(IEnumerable<K> other) {
            foreach (K key in other) {
                this.Remove(key);
            }
        }

        /// <summary>
        /// Modifies the current TrieSet to contain only elements that are also in the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current TrieSet.</param>
        public void IntersectWith(IEnumerable<K> other) {
            HashSet<K> set = other.ToHashSet();
            foreach (K key in this.ToArray()) {
                if (!set.Contains(key)) {
                    this.Remove(key);
                }
            }
        }

        /// <summary>
        /// Determines whether the current set is a proper subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// true if the current set is a proper subset of the specified collection;
        /// otherwise, false.
        /// </returns>
        public bool IsProperSubsetOf(IEnumerable<K> other) {
            return other.ToHashSet().IsProperSupersetOf(this);
        }

        /// <summary>
        /// Determines whether the current TrieSet is a proper superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current TrieSet.</param>
        /// <returns>True if the current TrieSet is a proper superset of the specified collection; otherwise, false.</returns>
        public bool IsProperSupersetOf(IEnumerable<K> other) {
            return other.ToHashSet().IsProperSubsetOf(this);
        }

        /// <summary>
        /// Determines whether the current set is a subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        /// True if the current set is a subset of the specified collection; otherwise, false.
        /// </returns>
        public bool IsSubsetOf(IEnumerable<K> other) {
            return other.ToHashSet().IsSupersetOf(this);
        }

        /// <summary>
        /// Determines whether the current <see cref="TrieSet{K, T}"/> object is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection of keys to compare with the current set.</param>
        /// <returns>
        /// true if the current set is a superset of the specified collection; otherwise, false.
        /// </returns>
        public bool IsSupersetOf(IEnumerable<K> other) {
            return other.ToHashSet().IsSubsetOf(this);
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">An <see cref="IEnumerable{T}"/> containing the elements to compare with the current set.</param>
        /// <returns>True if the current set and the specified collection share at least one common element; otherwise, false.</returns>
        public bool Overlaps(IEnumerable<K> other) {
            return other.ToHashSet().Overlaps(this);
        }

        /// <summary>
        /// Determines whether the current set and a specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>True if the current set is equal to the specified collection; otherwise, false.</returns>
        public bool SetEquals(IEnumerable<K> other) {
            return other.ToHashSet().SetEquals(this);
        }

        /// <summary>
        /// Modifies the current TrieSet to contain only elements that are in either the current TrieSet
        /// or the specified collection, but not both. This operation effectively represents
        /// the symmetric difference of two sets.
        /// </summary>
        /// <param name="other">The collection to compare to the current trie set. Any elements present in both sets
        /// will be removed from the current TrieSet, and elements in the collection
        /// but not in the TrieSet will be added.</param>
        public void SymmetricExceptWith(IEnumerable<K> other) {
            HashSet<K> set = other.ToHashSet();
            foreach (K key in set) {
                if (!this.Contains(key)) {
                    this.Add(key);
                } else {
                    this.Remove(key);
                }
            }
        }

        /// <summary>
        /// Modifies the current set to include all elements that are present in
        /// either the current set or the specified collection.
        /// </summary>
        /// <param name="other">The collection of keys to union with the current set.</param>
        public void UnionWith(IEnumerable<K> other) {
            foreach (K key in other) {
                this.Add(key);
            }
        }

        /// <summary>
        /// Adds the specified key to the trie set. If the key is successfully added,
        /// the trie set's count will increase, and the key will be available for retrieval.
        /// </summary>
        /// <param name="item">The key to be added to the trie set. The key must be non-null and iterable.</param>
        /// <returns>True if the key was successfully added; otherwise, false
        /// (e.g., if the key already exists in the trie set).</returns>
        bool ISet<K>.Add(K item) {
            if (this.Contains(item)) {
                return false;
            }
            
            this.Add(item);
            return true;
        }

        void ICollection<K>.Add(K item) {
            if (item is null) {
                return;
            }
            
            this.Add(item);
        }

        /// <summary>
        /// Removes all keys and elements from the TrieSet, resetting it to an empty state.
        /// </summary>
        public void Clear() {
            this.Root.Clear();
        }

        /// <summary>
        /// Checks whether the specified key exists in the trie set.
        /// Note that this is different from <see cref="ContainsPrefix"/>.
        /// A proper prefix of an existing key is not considered as present!
        /// </summary>
        /// <param name="key">The key to search for in the trie set.</param>
        /// <returns>True if the key exists in the trie set; otherwise, false.</returns>
        public bool Contains(K key) {
            return key != null && this.Root.HasKey(
                key.GetEnumerator(), this.HasSeparator, this.Separator, out Node _
            );
        }

        /// <summary>
        /// Determines whether the trie set contains any keys with the given prefix.
        /// Unlike <see cref="Contains"/>, this method checks if the specified prefix matches the beginning
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
            return prefix != null && this.Root.HasPrefix(
                prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Node _
            );
        }

        public IEnumerable<K> PrefixSearch(IEnumerable<T> prefix) {
            if (prefix is null) {
                return Enumerable.Empty<K>();
            }
            
            return this.Root.HasPrefix(prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Node node) 
                    ? node.AllChildKeys() 
                    : Enumerable.Empty<K>();
        }

        public bool RemoveAllWithPrefix(IEnumerable<T> prefix) {
            return prefix is not null && this.Root.HasPrefix(
                prefix.GetEnumerator(), this.HasSeparator, this.Separator, out Node last
            ) && last.Clear();
        }
        
        public bool Remove(IEnumerable<T> key) {
            return key is not null && this.Root.Remove(key.GetEnumerator());
        }

        public IEnumerable<K> Enumerate() {
            return this.Root.AllChildKeys();
        }

        public void CopyTo(K[] array, int arrayIndex) {
            this.Root.AllChildKeys().ToArray().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified key from the TrieSet if it exists.
        /// </summary>
        /// <param name="key">The key to be removed from the TrieSet.</param>
        /// <returns>Returns true if the key was successfully removed, otherwise false.</returns>
        public bool Remove(K key) {
            return key is not null && this.Root.Remove(key.GetEnumerator());
        }
    }
}
