using Events;
using InteractionSystem.Runtime;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map.Noticeboard
{
    public sealed class MapNoticeboard : Interactable
    {
        public MapUnlockRequirementSO mapUnlockRequirementSO;
        public TilemapCollider2D mapTileCollider2D;
        public CrossObjectEventWithDataSO broadcastNoticeboardUnlockRequirement;
        public Canvas promptCanvas;
        private bool isInteracted = false;

        void Start()
        {
            this.promptCanvas.gameObject.SetActive(false);
        }

        public void Interact()
        {
            if (!this.isInteracted)
            {
                this.isInteracted = true;
                this.broadcastNoticeboardUnlockRequirement.TriggerEvent(this, this.mapUnlockRequirementSO);
            }
        }
        
    }
}
