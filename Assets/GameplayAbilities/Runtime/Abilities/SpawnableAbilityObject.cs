using System.Collections;
using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using DataStructuresForUnity.Runtime.ObjectPooling;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    [RequireComponent(typeof(BoundingRect))]
    public class SpawnableAbilityObject : PoolableObject, ISpawnable<AbilityData> {
        private List<IActivatable> Objects { get; } = new List<IActivatable>();
        private BoundingRect BoundingRect { get; set; }
        [field: SerializeField] private string Id { get; set; }
        [field: SerializeField] private BoundingRect.Alignment Alignment { get; set; }
        
        private AbilityData Data { get; set; }
        public override string PoolableId => this.Id;
        private bool IsActive => this.Objects.TrueForAll(obj => obj.IsActive);
        
        protected virtual void Awake() {
            this.GetComponentsInChildren(this.Objects);
            this.BoundingRect = this.GetComponent<BoundingRect>();
        }
        
        private IEnumerator AlignToParentAndPlay(AbilityData data) {
            Transform parent = this.transform.parent;
            if (parent) {
                BoundingRect parentRect = parent.GetComponentInParent<BoundingRect>();
                if (parentRect) {
                    yield return new WaitForFixedUpdate();
                    this.BoundingRect.ResizeTo(parentRect);
                    yield return new WaitForFixedUpdate();
                    this.BoundingRect.AlignTo(parentRect, this.Alignment);
                }
            }
            
            foreach (IActivatable obj in this.Objects) {
                obj.Activate();
            }
            
            this.OnJustActivated(data);
            yield return new WaitForSeconds(data.Info.Duration);
            this.Objects.ForEach(obj => obj.Deactivate());
            yield return new WaitUntil(() => this.Objects.TrueForAll(obj => !obj.IsActive));
            this.Destroy();
        }

        public virtual void OnJustActivated(AbilityData data) { }

        public void Activate(AbilityData data) {
            this.Data = data;
            this.StartCoroutine(this.AlignToParentAndPlay(data));
        }
        
        public virtual void OnActive(AbilityData data) { }

        public void Destroy() {
            this.Return();
        }

        private void Update() {
            if (this.IsActive) {
                this.OnActive(this.Data);
            }
        }
    }
}
