using System.Collections.Generic;
using ModularItemsAndInventory.Runtime.Inventory;

namespace ModularItemsAndInventory.Runtime.Items.Orderings {
    /// <summary>
    /// A collection of useful orderings based on item keys.
    /// </summary>
    public static partial class ItemOrdering {
        public static IComparer<ItemKey> Default { get; } = Comparer<ItemKey>.Create((x, y) => x.CompareTo(y));

        public static IComparer<ItemKey> DefaultReverse { get; } = Comparer<ItemKey>.Create((x, y) => y.CompareTo(x));

        public static IComparer<ItemKey> ByName { get; } =
            Comparer<ItemKey>.Create((x, y) => string.CompareOrdinal(x.Name, y.Name));
        
        public static IComparer<ItemKey> ByNameReverse { get; } =
            Comparer<ItemKey>.Create((x, y) => string.CompareOrdinal(y.Name, x.Name));
        
        public static IComparer<ItemKey> ByType { get; } =
            Comparer<ItemKey>.Create((x, y) => ItemDatabase.TypeOf(x).CompareTo(ItemDatabase.TypeOf(y)));
        
        public static IComparer<ItemKey> ByTypeReverse { get; } =
            Comparer<ItemKey>.Create((x, y) => ItemDatabase.TypeOf(y).CompareTo(ItemDatabase.TypeOf(x)));
    }
}
