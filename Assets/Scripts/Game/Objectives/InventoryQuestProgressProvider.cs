using ModularItemsAndInventory.Runtime.Inventory;
using QuestAndObjective.Runtime;

namespace Game.Objectives {
    public sealed class InventoryQuestProgressProvider : QuestProgressProvider<Inventory> {
        public InventoryQuestProgressProvider(Inventory source) : base(source) { }

        public override bool HasValue(string variableName, out int value) {
            value = this.Source.Count(variableName.Split(':')[0]);
            return value > 0 || base.HasValue(variableName, out value);
        }
    }
}
