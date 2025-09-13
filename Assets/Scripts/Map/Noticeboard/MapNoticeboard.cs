using UnityEngine;
using UnityEngine.Tilemaps;
using Player;
using UnityEngine.UI;
using Events;
using InteractionSystem.Runtime;

namespace Map
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
            promptCanvas.gameObject.SetActive(false);
        }

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
