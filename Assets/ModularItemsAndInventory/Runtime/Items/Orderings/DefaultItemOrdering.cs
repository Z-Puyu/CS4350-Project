namespace ModularItemsAndInventory.Runtime.Items.Orderings {
    public sealed class DefaultItemOrdering : ItemOrdering {
        protected override int CompareExistingItems(Item x, Item y) {
            return x.CompareTo(y);
        }
    }
}
