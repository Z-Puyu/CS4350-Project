using System.Collections;
using System.Collections.Generic;
using Events;
using Game.Enemies;
using Map.RegionBorder;
using Map.Wave_manager;
using UnityEngine;

namespace Purgatory_system
{
    public class PurgatoryWaveManager : MonoBehaviour
    {
        [SerializeField] private RegionBorder purgatoryMap;
        [SerializeField] private Transform playerSpawnPosition;
        [SerializeField] private List<Vector2> spawnPositions;
        [SerializeField] private Enemy ghostPrefab;
        
        [SerializeField] private int currentGaugeAmount;
        [SerializeField] private int requiredGaugeAmount;
        [SerializeField] private WaveManager waveManager;
        
        [SerializeField] private PurgatoryUIManager purgatoryUIManager;
        [SerializeField] private CrossObjectEventSO revivePlayer;
        
        private int spawnIndex;

        void Start()
        {
            ResetGauge();
            purgatoryUIManager = GetComponent<PurgatoryUIManager>();
            purgatoryUIManager.ResetGauge();
            Vector2 origin = playerSpawnPosition.position;
            Vector2 topLeft = new Vector2(origin.x + 10, origin.y - 10);
            Vector2 bottomLeft = new Vector2(origin.x + 10, origin.y + 10);
            Vector2 topRight = new Vector2(origin.x - 10, origin.y - 10);
            Vector2 bottomRight = new Vector2(origin.x - 10, origin.y + 10);
            spawnPositions = new List<Vector2>(){topLeft, topRight, bottomRight, bottomLeft};
        }

        public void PlayerDie()
        {
            StartCoroutine(SpawnDelay());
        }

        IEnumerator SpawnDelay()
        {
            yield return new WaitForSeconds(5.0f);
            for (int i = 0; i < 4; i++)
            {
                Instantiate(ghostPrefab, spawnPositions[i], Quaternion.identity);       
            }
        }

        public void SpawnEnemy()
        {
            Instantiate(ghostPrefab, spawnPositions[spawnIndex], Quaternion.identity);
            spawnIndex++;
            spawnIndex %= 4;
        }

        void ResetGauge()
        {
            currentGaugeAmount = 0;
            requiredGaugeAmount = (int) Mathf.Pow(2,waveManager.GetWave());
        }

        public void FillGauge()
        {
            currentGaugeAmount += 1;
            purgatoryUIManager.UpdateGauge((float)(currentGaugeAmount) / (float)requiredGaugeAmount);
            if (currentGaugeAmount == requiredGaugeAmount)
            {
                revivePlayer.TriggerEvent();
                purgatoryUIManager.HideCanvas();
                ResetGauge();
            }
        }
    }
}
