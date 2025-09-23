using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Map {
    [DisallowMultipleComponent, RequireComponent(typeof(Tilemap))]
    public sealed class Region : MonoBehaviour, IMap {
        [field: SerializeField] private Transform DefaultSpawnPoint { get; set; }

        private Vector3Int DefaultSpawnPointCoordinate { get; set; }
        private Tilemap Tilemap { get; set; }
        public Vector3Int Origin => this.DefaultSpawnPointCoordinate;
        
        private void Awake() {
            this.Tilemap = this.GetComponent<Tilemap>();
            this.DefaultSpawnPointCoordinate = this.Tilemap.WorldToCell(this.DefaultSpawnPoint.position);
        }

        public void PlaceObject(GameObject obj, Vector3Int coordinates) {
            obj.transform.position = this.Tilemap.GetCellCenterWorld(coordinates);
        }
        
        public bool HasCellAt(Vector3 worldPosition, out Vector3Int coordinates) {
            coordinates = this.Tilemap.WorldToCell(worldPosition);
            return this.Tilemap.HasTile(coordinates);
        }

        public Vector3 CellToWorld(Vector3Int coordinates) {
            return this.Tilemap.GetCellCenterWorld(coordinates);
        }
        
        public Vector3Int WorldToCell(Vector3 position) {
            return this.Tilemap.WorldToCell(position);
        }
    }
}
