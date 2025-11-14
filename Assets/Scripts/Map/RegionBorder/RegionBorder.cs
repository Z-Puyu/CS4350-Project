using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map.RegionBorder
{
    public class RegionBorder : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        private Tilemap childTilemap;
        private BoxCollider2D _boxCollider2D;
        
        void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            childTilemap = transform.GetChild(0).GetComponentInChildren<Tilemap>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            tilemap = GetComponent<Tilemap>();
            if (_boxCollider2D == null)
            {
                return;
            } 
            _boxCollider2D.isTrigger = true;
            if (!gameObject.CompareTag("Main map") && !gameObject.CompareTag("Purgatory"))
            {
                _boxCollider2D.isTrigger = false;
                tilemap.color = new Color(0.3f, 0.3f, 0.3f);
                childTilemap.color = new Color(0.3f, 0.3f, 0.3f);
            }
        }

        public List<float> GetEdgeExtremeValues()
        {
            BoundsInt bounds = tilemap.cellBounds;
            return new List<float> { bounds.xMin, bounds.yMin, bounds.xMax, bounds.yMax };
        }

        public void UnlockRegion()
        {
            tilemap.color = Color.white;
            childTilemap.color = Color.white;
        }

        public bool GetMapIsUnlocked()
        {
            if (_boxCollider2D == null)
            {
                return false;
            } 
            return _boxCollider2D.isTrigger;
        }
    }
}
