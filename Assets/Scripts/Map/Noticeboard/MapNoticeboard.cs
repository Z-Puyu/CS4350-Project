using UnityEngine;
using UnityEngine.Tilemaps;
using Events;
using InteractionSystem.Runtime;

namespace Map
{
    public sealed class MapNoticeboard : Interactable
    {
        public MapUnlockRequirementSO mapUnlockRequirementSO;
        public CrossObjectEventWithDataSO broadcastNoticeboardUnlockRequirement;
        private bool isInteracted = false;

        public void Interact()
        {
            if (!isInteracted)
            {
                isInteracted = true;
                broadcastNoticeboardUnlockRequirement.TriggerEvent(this, mapUnlockRequirementSO);
            }
        }
        
    }
}
