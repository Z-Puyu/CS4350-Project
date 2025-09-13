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
        private bool isInteracted = true;

        void Start()
        {
            promptCanvas.gameObject.SetActive(false);
        }

        public void Interact()
        {
            if (!isInteracted)
            {
                broadcastNoticeboardUnlockRequirement.TriggerEvent(this, mapUnlockRequirementSO);
            }
        }

        // void OnTriggerEnter2D(Collider2D collider)
        // {
        //     PlayerInteract playerInteract = collider.GetComponent<PlayerInteract>();
        //     if (playerInteract)
        //     {
        //         promptCanvas.gameObject.SetActive(true);
        //         //playerInteract.SetInteractableObject(this);
        //     }
        // }
        //
        // void OnTriggerExit2D(Collider2D collider)
        // {
        //     PlayerInteract playerInteract = collider.GetComponent<PlayerInteract>();
        //     if (playerInteract)
        //     {
        //         promptCanvas.gameObject.SetActive(false);
        //         //playerInteract.SetInteractableObject(null);
        //     }
        // }
    }
}
