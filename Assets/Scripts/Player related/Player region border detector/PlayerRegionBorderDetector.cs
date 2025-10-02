using Events;
using Map.RegionBorder;
using Map.Wave_manager;
using UnityEngine;

namespace Player_related.Player_region_border_detector
{
    public class PlayerRegionBorderDetector : MonoBehaviour
    {
        public CrossObjectEventWithDataSO broadcastMyCurrentMap;
        [SerializeField] private WaveManager waveManager;
        private void OnTriggerEnter2D(Collider2D other)
        {
            RegionBorder regionBorder = other.gameObject.GetComponent<RegionBorder>();
            if (regionBorder != null)
            {
                this.waveManager.SetCurrentRegion(regionBorder);
            }
        }
    }   
}
