using ModularItemsAndInventory.Runtime.Inventory;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModularItemsAndInventory.Runtime.Items {
    [DisallowMultipleComponent]
    public sealed class PickUp : MonoBehaviour {
        [field: SerializeField] private ItemData ItemData { get; set; }
        [field: SerializeField, Required] private Collider2D Collider { get; set; }
        private ItemKey Item { get; set; }
        [field: SerializeField, MinValue(1)] private int Count { get; set; } = 1;

        private void Awake() {
            if (!this.ItemData) {
                Object.Destroy(this);
            } else {
                this.Item = ItemKey.From(this.ItemData);
            }
        }

        internal PickUp With(int count, ItemKey item) {
            this.Count = count;
            this.Item = item;
            return this;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (this.Count <= 0 || !other.TryGetComponent(out ICollector collector)) {
                return;
            }

            collector.Collect(this.Count, this.Item);
            Object.Destroy(this.gameObject);
        }

        public override string ToString() {
            return $"Pick-up of {this.Item} × {this.Count}";
        }
    }
}
