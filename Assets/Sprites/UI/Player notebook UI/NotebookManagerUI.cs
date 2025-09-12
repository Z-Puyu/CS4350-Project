using UnityEngine;
using System.Collections.Generic;
using Map;

public class NotebookManagerUI : MonoBehaviour
{
    private List<MapUnlockRequirementSO> allRequirementSO;

    public void AddRequirementSO(Component component, object rso)
    {
        MapUnlockRequirementSO mapUnlockRequirementSO = (MapUnlockRequirementSO)((object[])rso)[0];
        allRequirementSO.Add(mapUnlockRequirementSO);
    }
}
