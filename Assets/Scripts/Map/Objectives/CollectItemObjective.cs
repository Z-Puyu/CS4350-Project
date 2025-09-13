using System.Collections.Generic;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

namespace Map.Objectives
{
    [CreateAssetMenu(fileName = "Collect items objective", menuName = "Objectives/Collect items objective")]
    public class CollectItemObjective : Objective
    {
        public ItemData itemToCollect;
        public int itemToCollectAmount;
        [SerializeField] private int currentItemCounter;

        public void AddProgress(ItemData itemToCollect, List<CollectItemObjective> allCollectItesmObjectives) {
            if (this.itemToCollect == itemToCollect)
            {
                currentItemCounter += 1;
            }

            if (currentItemCounter >= itemToCollectAmount)
            {
                allCollectItesmObjectives.Remove(this);
            }
        }
    }
}