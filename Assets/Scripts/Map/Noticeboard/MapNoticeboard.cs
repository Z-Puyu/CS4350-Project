using System.Collections;
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
        [field: SerializeField] 
            private Animator animator;
        [field: SerializeField] 
        private GameObject bossIndicator;
        [field: SerializeField] 
        private UnityEvent CanInteractAgainEvent { get; set; }
        public MapUnlockRequirementSO mapUnlockRequirementSO;
        public CrossObjectEventWithDataSO broadcastNoticeboardUnlockRequirement;
        public CrossObjectEventWithDataSO broadcastNoticeboardBossSpawnt;
        private bool isInteracted = false;
        private bool isActivated = false;

        void Start()
        {
            bossIndicator.SetActive(false);
        }

        public void Interact()
        {
            if (!isInteracted)
            {
                animator.SetTrigger("collect");
                isInteracted = true;
                broadcastNoticeboardUnlockRequirement.TriggerEvent(this, mapUnlockRequirementSO);
            } else if (isInteracted && !isActivated)
            {
                animator.SetTrigger("activate");
                OnInteractAgainEvent?.Invoke();
            }
        }

        public void OnInteractAgain()
        {
            isActivated = true;
            broadcastNoticeboardBossSpawnt.TriggerEvent(mapUnlockRequirementSO.bossToSpawn, mapUnlockRequirementSO.bossToSpawn.GetEnemyData().messageForPlayerBeforeSpawn);
        }

        public void VerifyObjectiveCompleted(Component component, object mapSO)
        {
            MapUnlockRequirementSO unlockedRequirement = (MapUnlockRequirementSO)((object[])mapSO)[0];
            if (unlockedRequirement == mapUnlockRequirementSO)
            {
                bossIndicator.SetActive(true);
                CanInteractAgainEvent?.Invoke();
            }
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }
        
    }
}
