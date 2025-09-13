using Map.Objectives;
using UnityEngine;
using System.Collections.Generic;

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
}
