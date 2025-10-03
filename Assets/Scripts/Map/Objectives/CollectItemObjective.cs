using System.Collections.Generic;
using Map.Objectives.Objective_UI;
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
        
        void OnEnable()
        {
            currentItemCounter = 0;
        }

        public void AddProgress(ObjectiveManager objectiveManager, ItemData itemToCollect, List<CollectItemObjective> allCollectItesmObjectives) {
            if (this.itemToCollect == itemToCollect)
            {
                currentItemCounter += 1;
            }

            if (currentItemCounter >= itemToCollectAmount)
            {
                allCollectItesmObjectives.Remove(this);
                objectiveManager.HandleDeletion(this);
            }
        }
        
        public override bool IsComplete()
        {
            return currentItemCounter >= itemToCollectAmount;
        }
    }
}