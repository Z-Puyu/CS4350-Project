using Map.Objectives;
using UnityEngine;
using System.Collections.Generic;
using ModularItemsAndInventory.Runtime.Items;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private List<KillEnemiesObjective> killEnemiesObjectives;
    [SerializeField] private List<CollectItemObjective> collectItesmObjectives;

    public void AddEnemyToObjective()
    {
        foreach (var objective in killEnemiesObjectives)
        {
            objective.AddProgress();
        }
    }

    public void AddItemToObjective(Component component, object item)
    {
        Item itemToAdd = (Item)((object[])item)[0];
        foreach (var objective in collectItesmObjectives)
        {
            objective.AddProgress(itemToAdd);
        }
    }
    

}
