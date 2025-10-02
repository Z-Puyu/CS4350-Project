using UnityEngine;
using System.Collections.Generic;

namespace Map.Objectives.Objective_UI
{
    public class ObjectiveUIManager : MonoBehaviour
    {
        [SerializeField] private MapUnlockRequirementPanel mapUnlockRequirementPanelPrefab;
        [SerializeField] private Transform spawnContent;
        [SerializeField] private GameObject backdrop;
        [SerializeField] private List<MapUnlockRequirementPanel> spawnedPanels;
        
        public void OpenUI()
        {
            Reset();
            backdrop.SetActive(true);    
        }
        
        public void SpawnPanel(MapUnlockRequirementSO requirementSO, bool isSelected)
        {
            GameObject spawnedPanel = Instantiate(mapUnlockRequirementPanelPrefab.gameObject, spawnContent);
            MapUnlockRequirementPanel component = spawnedPanel.GetComponent<MapUnlockRequirementPanel>();
            spawnedPanels.Add(component);
            component.SetMapUnlockRequirement(requirementSO, isSelected);
        }

        public void Reset()
        {
            foreach (var spawnedPanel in spawnedPanels)
            {
                spawnedPanel.Reset();
            }
            spawnedPanels.Clear();
        }
    }   
}
