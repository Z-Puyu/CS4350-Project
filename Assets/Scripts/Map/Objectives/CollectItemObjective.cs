using ModularItemsAndInventory.Runtime.Items;

namespace Map.Objectives
{
    public class CollectItemObjective : Objective
    {
        public Item itemToCollect;
        public int itemToCollectAmount;
        private int currentItemCounter;

        public void AddProgress(Item itemToCollect) {
            if (this.itemToCollect == itemToCollect)
            {
                currentItemCounter += 1;
            }
        }
    }
}