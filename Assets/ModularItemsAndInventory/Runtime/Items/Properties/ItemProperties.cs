using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    /// <summary>
    /// Encapsulates a collection of properties associated with an item.
    /// Provides functionality to manage, query, and manipulate these properties.
    /// Implements <see cref="IEquatable{T}"/> for equality comparisons and <see cref="IComparable{T}"/> for sorting.
    /// </summary>
    public sealed class ItemProperties : IEquatable<ItemProperties>, IComparable<ItemProperties> {
        private Item OwningItem { get; }

        private Dictionary<Type, IItemProperty> Properties { get; } =
            new Dictionary<Type, IItemProperty>();

        /// <summary>
        /// Applies the item properties to the specified target.
        /// Iterates through all properties contained in this instance, and if a property implements
        /// the <see cref="IItemProperty{T}"/> interface for the specified type, it invokes
        /// the property-specific processing logic.
        /// </summary>
        /// <typeparam name="T">The target type to which the item properties are applied.</typeparam>
        /// <param name="target">The target instance that will have the properties applied to it.</param>
        public void Affect<T>(T target) {
            foreach (IItemProperty p in this.Properties.Values) {
                if (p is IItemProperty<T> property) {
                    property.Process(this.OwningItem, target);
                }
            }
        }

        /// <summary>
        /// Determines if the item properties collection contains a property of the exact type as the specified type.
        /// If found, the property is returned via the output parameter.
        /// </summary>
        /// <typeparam name="P">The type of the property to search for, which must implement <see cref="IItemProperty"/>.</typeparam>
        /// <param name="property">The output parameter that will contain the property, if found.</param>
        /// <returns>
        /// Returns <c>true</c> if exactly one property of the specified type is found; otherwise, <c>false</c>.
        /// </returns>
        public bool HaveExactly<P>(out P property) where P : IItemProperty {
            if (this.Properties.TryGetValue(typeof(P), out IItemProperty p)) {
                property = (P)p;
                return true;
            }

            property = default;
            return false;
        }

        /// <summary>
        /// Determines whether the specified property is present in the collection.
        /// Checks for the exact type match of the property in the collection.
        /// </summary>
        /// <typeparam name="P">The type of the property to be checked.</typeparam>
        /// <returns>True if the specified property type exists in the collection; otherwise, false.</returns>
        public bool HaveExactly<P>() where P : IItemProperty {
            return this.Properties.ContainsKey(typeof(P));
        }

        /// <summary>
        /// Determines whether the collection contains any property that is a subtype of <typeparamref name="P"/>.
        /// </summary>
        /// <typeparam name="P">The type of the property to check for in the collection.</typeparam>
        /// <returns>True if a property of type <typeparamref name="P"/> or a compatible type exists in the collection;
        /// otherwise, false.</returns>
        public bool Have<P>() where P : IItemProperty {
            Type type = typeof(P);
            return this.Properties.Keys.Any(k => type.IsAssignableFrom(k));
        }

        /// <summary>
        /// Determines if the collection contains properties of the specified type and retrieves them.
        /// </summary>
        /// <typeparam name="P">The type of properties to search for,
        /// which must implement <see cref="IItemProperty"/>.</typeparam>
        /// <param name="properties">When this method returns, contains the collection of properties
        /// with the specified type, if found; otherwise, an empty collection.</param>
        /// <returns>True if properties of the specified type exist in the collection; otherwise, false.</returns>
        public bool Have<P>(out IEnumerable<P> properties) where P : IItemProperty {
            properties = this.Properties.Values.OfType<P>();
            return properties.Any();
        }

        private ItemProperties(Item owningItem) {
            this.OwningItem = owningItem;
        }

        public static ItemProperties Of(Item item) {
            return new ItemProperties(item);
        }

        internal ItemProperties With(IItemProperty property) {
            Type type = property.GetType();
            if (this.Properties.ContainsKey(type)) {
                Debug.LogWarning($"Duplicate property {property.GetType()} will be ignored.");
            } else {
                this.Properties.Add(type, property.Instantiate());
            }

            return this;
        }

        internal ItemProperties With(IEnumerable<IItemProperty> props) {
            foreach (IItemProperty p in props) {
                Type type = p.GetType();
                if (this.Properties.ContainsKey(type)) {
                    Debug.LogWarning($"Duplicate property {p.GetType()} will be ignored.");
                } else {
                    this.Properties.Add(type, p.Instantiate());
                }
            }
            
            return this;
        }
        
        public bool Equals(ItemProperties other) {
            return other != null && this.Properties.Values.ToHashSet().SetEquals(other.Properties.Values);
        }

        public int CompareTo(ItemProperties other) {
            int length = Math.Min(this.Properties.Count, other.Properties.Count);
            List<IItemProperty> otherProperties = other.Properties.Values.ToList();
            otherProperties.Sort();
            List<IItemProperty> thisProperties = this.Properties.Values.ToList();
            thisProperties.Sort();
            for (int i = 0; i < length; i += 1) {
                IItemProperty thisProperty = thisProperties[i];
                IItemProperty otherProperty = otherProperties[i];
                int comparison = thisProperty.CompareTo(otherProperty);
                if (comparison != 0) {
                    return comparison;
                }
            }
            
            return this.Properties.Count - otherProperties.Count;
        }

        public override int GetHashCode() {
            return HashSet<IItemProperty>.CreateSetComparer().GetHashCode(this.Properties.Values.ToHashSet());
        }
    }
}
