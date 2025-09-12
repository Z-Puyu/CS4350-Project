using ModularItemsAndInventory.Runtime.Items;

namespace QuestAndObjective.Runtime {
    public readonly struct InventoryChangeEvent :IObjectiveStateUpdateEvent {
        public string ItemId { get; }
        public int ChangeInCount { get; }
        
        public InventoryChangeEvent(string itemId, int changeInCount) {
            this.ItemId = itemId;
            this.ChangeInCount = changeInCount;
        }
    }
}
