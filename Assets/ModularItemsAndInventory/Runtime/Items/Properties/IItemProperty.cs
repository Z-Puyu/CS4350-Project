using System;

namespace ModularItemsAndInventory.Runtime.Items.Properties {
    /// <summary>
    /// Represents a generic interface for item properties that can be instantiated.
    /// Support comparison-semantics to enable ordering between items which only differ in properties.
    /// </summary>
    public interface IItemProperty : IEquatable<IItemProperty>, IComparable<IItemProperty> {
        public IItemProperty Instantiate();
    }

    /// <summary>
    /// Represents a generic interface for item properties that can be instantiated.
    /// Support comparison-semantics to enable ordering between items which only differ in properties.
    /// </summary>
    public interface IItemProperty<in T> : IItemProperty {
        /// <summary>
        /// Processes an item used by a specific target. The implementation is determined
        /// by the concrete class inheriting from <see cref="ItemProperty{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The <see cref="Item"/> instance to be processed. This parameter is read-only.
        /// </param>
        /// <param name="target">
        /// A target of type <typeparamref name="T"/> that can be affected or interacted with during item usage.
        /// The type and behaviour depend on the specific implementation of the derived class.
        /// </param>
        public void Process(in Item item, T target);
    }
}
