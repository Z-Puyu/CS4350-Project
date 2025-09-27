using System;
using System.Collections;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [RequireComponent(typeof(BoundingRect))]
    public class SpawnableAbilityObject : PoolableObject, ISpawnable<AbilityData> {
        private List<IActivatable> Objects { get; } = new List<IActivatable>();
        private BoundingRect BoundingRect { get; set; }
        [field: SerializeField] private string Id { get; set; }
        [field: SerializeField] private BoundingRect.Alignment Alignment { get; set; }
        
        public override string PoolableId => this.Id;
        
        private void Awake() {
            this.GetComponentsInChildren(this.Objects);
            this.BoundingRect = this.GetComponent<BoundingRect>();
        }
        
        private IEnumerator AlignToParentAndPlay(int duration) {
            yield return new WaitForFixedUpdate();
            Transform parent = this.transform.parent;
            if (parent) {
                BoundingRect parentRect = parent.GetComponentInParent<BoundingRect>();
                if (parentRect) {
                    this.BoundingRect.AlignTo(parentRect, this.Alignment);
                }
            }
            
            foreach (IActivatable obj in this.Objects) {
                obj.Activate();
            }
            
            yield return new WaitForSeconds(duration);
            this.Objects.ForEach(obj => obj.Deactivate());
            yield return new WaitUntil(() => this.Objects.TrueForAll(obj => !obj.IsActive));
            this.Destroy();
        }

        public void Activate(AbilityData data) {
            this.StartCoroutine(this.AlignToParentAndPlay(data.Info.Duration));
        }

        public void Destroy() {
            this.Return();
        }
    }
}
