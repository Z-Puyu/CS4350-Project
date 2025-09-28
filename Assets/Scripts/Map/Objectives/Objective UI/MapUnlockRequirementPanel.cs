using UnityEngine;
using System.Collections.Generic;
using Events;
using TMPro;
using UnityEngine.UI;

namespace Map.Objectives.Objective_UI
{
    public class MapUnlockRequirementPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI objectiveTitle;
        [SerializeField] private ObjectiveText objectiveTextPrefab;
        [SerializeField] private Transform spawnContent;
        [SerializeField] private MapUnlockRequirementSO unlockRequirementSo;
        [SerializeField] private List<GameObject> spawnedObjectiveText = new List<GameObject>();

        [SerializeField] private Image panelImage;
        [SerializeField] private List<Sprite> possibleBackground;
        [SerializeField] private CrossObjectEventWithDataSO broadcastUnlockRequiremet;
        
        public void SetMapUnlockRequirement(MapUnlockRequirementSO unlockRequirementSo, bool isSelected)
        {
            if (isSelected)
            {
                
            }
            else
            {
                
            }
            this.unlockRequirementSo = unlockRequirementSo;
            panelImage.sprite = possibleBackground[Random.Range(0, possibleBackground.Count - 1)];
            this.unlockRequirementSo.SetTitle(objectiveTitle);
            this.unlockRequirementSo.SpawnObjectiveText(objectiveTextPrefab.gameObject, spawnContent, spawnedObjectiveText);
        }

        public void Reset()
        {
            foreach (var spawnedObjectiveText in spawnedObjectiveText)
            {
                Destroy(spawnedObjectiveText);
            }
            Destroy(this.gameObject);
        }

        public void BroadcastUnlockRequirement()
        {
            broadcastUnlockRequiremet.TriggerEvent(this, unlockRequirementSo);
        }
    }
}