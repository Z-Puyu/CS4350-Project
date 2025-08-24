using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModularItemsAndInventory.Runtime.Items.Orderings {
    /// <summary>
    /// Represents an abstract base class for defining custom item ordering logic.
    /// Provides a mechanism for comparing two items and determining their relative order.
    /// </summary>
    /// <remarks>
    /// The class implements the <see cref="IComparer{T}"/> interface for comparing <see cref="Item"/> instances.
    /// This ensures that derived classes provide a consistent way to order items based on specific criteria.
    /// </remarks>
    /// <example>
    /// Custom implementations should override the <see cref="CompareExistingItems(Item, Item)"/>
    /// method to define their specific ordering logic.
    /// </example>
    public abstract class ItemOrdering : IComparer<Item> {
        /// <summary>
        /// Represents the default implementation of the <see cref="ItemOrdering"/> class.
        /// </summary>
        /// <remarks>
        /// This instance applies the default logic for comparing and ordering items.
        /// It uses the <see cref="DefaultItemOrdering"/> class to define the standard behaviour.
        /// </remarks>
        /// <seealso cref="ItemOrdering"/>
        /// <seealso cref="DefaultItemOrdering"/>
        public static ItemOrdering Default = new DefaultItemOrdering();
        
        public int Compare(Item x, Item y) {
            if (object.ReferenceEquals(x, y)) {
                return 0;
            }

            if (x is null) {
                return -1;
            }

            return y is null ? 1 : this.CompareExistingItems(x, y);
        }

        protected abstract int CompareExistingItems([NotNull] Item x, [NotNull] Item y);
    }
}
