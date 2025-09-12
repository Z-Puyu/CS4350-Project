using ModularItemsAndInventory.Runtime.Items;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    public class GatherItemObjective : Objective<InventoryChangeEvent> {
        [field: SerializeField, Required] private ItemData Item { get; set; }
        [field: SerializeField, MinValue(1)] private int Count { get; set; } = 1;
        private uint CurrentCount { get; set; }
        public override bool IsCompleted => this.CurrentCount >= this.Count;

        protected override bool Advance(InventoryChangeEvent @event) {
            if (@event.ItemId != this.Item.Id || @event.ChangeInCount == 0) {
                return false;
            }
            
            if (@event.ChangeInCount < 0) {
                this.CurrentCount -= (uint)(-@event.ChangeInCount);
            } else {
                this.CurrentCount += (uint)@event.ChangeInCount;
            }
            
            return this.IsCompleted;
        }
    }
}
