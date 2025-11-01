using System.Collections.Generic;
using UnityEngine;

namespace Game.Map {
    [DisallowMultipleComponent, RequireComponent(typeof(Grid))]
    public class GameWorld : MonoBehaviour, IMap {
        private Grid Grid { get; set; }
        public List<Region> Regions { get; } = new List<Region>();
        public Vector3Int Origin => this.Regions.Count == 0 ? Vector3Int.zero : this.Regions[0].Origin;
        public Transform playerSpawnPositiob;
        
        private void Awake() {
            this.Grid = this.GetComponent<Grid>();
            this.GetComponentsInChildren(this.Regions);
        }

        public void TeleportPlayer(Transform playerTransform)
        {
            playerTransform.position = playerSpawnPositiob.position;
        }

        public void Unlock(Region region) {
            this.Regions.Add(region);
        }

        public bool HasCellAt(Vector3 worldPosition, out Vector3Int coordinates) {
            foreach (Region region in this.Regions) {
                if (!region.HasCellAt(worldPosition, out coordinates)) {
                    continue;
                }

                coordinates = this.WorldToCell(worldPosition);
                return true;
            }
            
            coordinates = Vector3Int.zero;
            return false;
        }

        public Vector3 CellToWorld(Vector3Int coordinates) {
            return this.Grid.CellToWorld(coordinates);
        }
        
        public Vector3Int WorldToCell(Vector3 position) {
            return this.Grid.WorldToCell(position);
        }
    }
}
