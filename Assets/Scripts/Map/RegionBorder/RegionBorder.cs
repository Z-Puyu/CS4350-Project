using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map.RegionBorder
{
    public class RegionBorder : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        private BoxCollider2D _boxCollider2D;
        
        void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _boxCollider2D.isTrigger = true;
            if (!gameObject.CompareTag("Main map"))
            {
                _boxCollider2D.isTrigger = false;
            }
            tilemap = GetComponent<Tilemap>();
        }

        public List<float> GetEdgeExtremeValues()
        {
            BoundsInt bounds = tilemap.cellBounds;
            return new List<float> { bounds.xMin, bounds.yMin, bounds.xMax, bounds.yMax };
        }

        public bool GetMapIsUnlocked()
        {
            return _boxCollider2D.isTrigger;
        }
    }
}
