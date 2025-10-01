using System;
using Events;
using Map.RegionBorder;
using Map.Wave_manager;
using UnityEngine;

namespace Player
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
                waveManager.SetCurrentRegion(regionBorder);
            }
        }
    }   
}
