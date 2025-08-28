using System;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    /// <summary>
    /// Encapsulate an immutable property defined in an asset.
    /// </summary>
    [Serializable]
    public abstract class ItemProperty : IItemProperty {
        private Lazy<string> CachedSortKey { get; }
        internal string SortKey => this.CachedSortKey.Value;

        protected ItemProperty() {
            this.CachedSortKey = new Lazy<string>(this.GenerateSortKey);
        }

        protected abstract string GenerateSortKey();

        public bool Equals(IItemProperty other) {
            return this.CompareTo(other) == 0;
        }
        
        public int CompareTo(IItemProperty other) {
            if (object.ReferenceEquals(this, other)) {
                return 0;
            }

            return other is null
                    ? 1
                    : string.CompareOrdinal(this.SortKey, ((ItemProperty)other).SortKey);
        }

        public abstract IItemProperty Instantiate();

        public abstract void Process(in Item item, GameObject target);

        public sealed override int GetHashCode() {
            return this.SortKey.GetHashCode();
        }
    }
}
