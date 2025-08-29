namespace ModularItemsAndInventory.Runtime.Items.Orderings {
    public sealed class ItemOrderingByType : ItemOrdering {
        protected override int CompareExistingItems(Item x, Item y) {
            return x.Type.CompareTo(y.Type);
        }
    }
}
