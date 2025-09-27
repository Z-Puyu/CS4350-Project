using UnityEngine;

namespace Game.Map {
    public interface IMap {
        public abstract Vector3Int Origin { get; }

        public void PlaceObjectAtOrigin(GameObject obj) {
            this.PlaceObject(obj, this.Origin);
        }
        
        public void PlaceObject(GameObject obj, Vector3Int coordinates);

        public bool HasCellAt(Vector3 worldPosition, out Vector3Int coordinates);
        
        /// <summary>
        /// Converts a cell coordinate to a world position.
        /// </summary>
        /// <param name="coordinates">A cell coordinates in the map's local space.</param>
        /// <returns>The world position of the cell.</returns>
        public Vector3 CellToWorld(Vector3Int coordinates);
        
        /// <summary>
        /// Converts a world position to a cell coordinate.
        /// </summary>
        /// <param name="position">A world position.</param>
        /// <returns>The cell coordinates in the map's local space.</returns>
        public Vector3Int WorldToCell(Vector3 position);
    }
}
