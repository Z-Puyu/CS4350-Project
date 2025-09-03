using System;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    /// <summary>
    /// Encapsulate an immutable property defined in an asset.
    /// </summary>
    [Serializable]
    public abstract class ItemProperty : IItemProperty {
        protected Lazy<string> CachedEncoding { get; }
        
        string IItemProperty.Encoding => this.CachedEncoding.Value;

        protected ItemProperty() {
            this.CachedEncoding = new Lazy<string>(this.Encode);
        }
        
        protected abstract string Encode();

        public bool Equals(IItemProperty other) {
            return this.CompareTo(other) == 0;
        }
        
        public int CompareTo(IItemProperty other) {
            if (object.ReferenceEquals(this, other)) {
                return 0;
            }

            return other is null ? 1 : string.CompareOrdinal(this.CachedEncoding.Value, other.Encoding);
        }
        
        public abstract IItemProperty Instantiate();

        public abstract void Process(in Item item, GameObject target);

        public sealed override int GetHashCode() {
            return this.CachedEncoding.Value.GetHashCode();
        }
    }
}
