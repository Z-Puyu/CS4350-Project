using ModularItemsAndInventory.Runtime.Items;

namespace ModularItemsAndInventory.Runtime.Inventory {
    public interface ICollector {
        public void Collect(int count, ItemKey item);
    }
}
