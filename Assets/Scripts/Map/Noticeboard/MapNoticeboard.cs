using UnityEngine;
using UnityEngine.Tilemaps;
using Events;
using InteractionSystem.Runtime;
using UnityEngine.Events;

namespace Map
{
    public sealed class MapNoticeboard : Interactable
    {
        [field: SerializeField]
        private UnityEvent OnInteractAgainEvent { get; set; }
        public MapUnlockRequirementSO mapUnlockRequirementSO;
        public CrossObjectEventWithDataSO broadcastNoticeboardUnlockRequirement;
        public CrossObjectEventWithDataSO broadcastNoticeboardBossSpawnt;
        private bool isInteracted = false;
        private bool isActivated = false;

        public void Interact()
        {
            if (!isInteracted)
            {
                isInteracted = true;
                broadcastNoticeboardUnlockRequirement.TriggerEvent(this, mapUnlockRequirementSO);
            } else if (isInteracted && !isActivated)
            {
                OnInteractAgainEvent?.Invoke();
            }
        }

        public void OnInteractAgain()
        {
            isActivated = true;
            broadcastNoticeboardBossSpawnt.TriggerEvent(this, mapUnlockRequirementSO.bossToSpawn);
        }
        
    }
}
