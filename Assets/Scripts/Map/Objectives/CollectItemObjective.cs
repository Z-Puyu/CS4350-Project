using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

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