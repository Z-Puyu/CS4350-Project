using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map.RegionBorder
{
    public class RegionBorder : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        void Awake()
        {
            tilemap = GetComponent<Tilemap>();
        }

        public List<float> GetEdgeExtremeValues()
        {
            BoundsInt bounds = tilemap.cellBounds;
            return new List<float> { bounds.xMin, bounds.yMin, bounds.xMax, bounds.yMax };
        }
    }
}
