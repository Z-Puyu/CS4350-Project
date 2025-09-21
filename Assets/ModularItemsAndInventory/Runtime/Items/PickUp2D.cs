using ModularItemsAndInventory.Runtime.Inventory;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModularItemsAndInventory.Runtime.Items {
    [DisallowMultipleComponent]
    public sealed class PickUp2D : MonoBehaviour {
        [field: SerializeField] private ItemData ItemData { get; set; }
        [field: SerializeField, Required] private Collider2D Collider { get; set; }
        private ItemKey Item { get; set; }
        [field: SerializeField, MinValue(1)] private int Count { get; set; } = 1;
        [field: SerializeField, Required] private SpriteRenderer SpriteRenderer { get; set; }
        private void Awake() {
            if (!this.ItemData) {
                return;
            }

            this.Item = ItemKey.From(this.ItemData);
            this.SpriteRenderer.sprite = ItemDatabase.IconOf(this.Item);
        }

        public PickUp2D With(int count, ItemKey item) {
            this.Count = count;
            this.Item = item;
            this.SpriteRenderer.sprite = ItemDatabase.IconOf(item);
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
