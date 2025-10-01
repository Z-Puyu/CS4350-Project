using System.Collections.Generic;
using System.Linq;
using Game.Enemies;
using UnityEngine;
using SaintsField;

namespace Map.Wave_manager
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private int wave;
        [SerializeField] private SaintsDictionary<int, SaintsDictionary<Enemy, int>> waveToEnemySpawner;
        [SerializeField] private RegionBorder.RegionBorder initialMap;
        [SerializeField] private RegionBorder.RegionBorder currentMap;
        [SerializeField] private float minX;
        [SerializeField] private float minY;
        [SerializeField] private float maxX;
        [SerializeField] private float maxY;
        
        void Start()
        {
            SetBoundary(initialMap);
            currentMap = initialMap;
        }

        public void SetCurrentRegion(RegionBorder.RegionBorder regionBorder)
        {
            currentMap = regionBorder;
            SetBoundary(regionBorder);
        }

        public void SetBoundary(RegionBorder.RegionBorder mapBorder)
        {
            List<float> coordinateList = mapBorder.GetEdgeExtremeValues();
            minX = coordinateList[0];
            minY = coordinateList[1];
            maxX = coordinateList[2];
            maxY = coordinateList[3];
        }

        public void SpawnEnemy()
        {
            Vector2 topLeft = new Vector2(minX, maxY);
            Vector2 bottomLeft = new Vector2(minX, minY);
            Vector2 topRight = new Vector2(maxX, maxY);
            Vector2 bottomRight = new Vector2(maxX, minY);
            List<Vector2> spawnPoints = new List<Vector2>(){topLeft, topRight, bottomRight, bottomLeft};
            SaintsDictionary<Enemy, int> enemiesForThisWave = waveToEnemySpawner[wave];
            List<Enemy> allEnemyDataForThisWave = enemiesForThisWave.Keys.ToList();
            List<int> counter = enemiesForThisWave.Values.ToList();
            int spawnIndex = 0;
            while (true)
            {
                bool isEnemyStillSpawning = false;
                for (int i = 0; i < allEnemyDataForThisWave.Count; i++)
                {
                    if (counter[i] > 0)
                    {
                        isEnemyStillSpawning = true;
                        Instantiate(allEnemyDataForThisWave[i], spawnPoints[spawnIndex], Quaternion.identity);
                        counter[i] -= 1;
                        spawnIndex++;
                        spawnIndex %= 4;   
                    }
                }
                if (!isEnemyStillSpawning)
                {
                    break;
                }
            }
            
        }

        public void ClearWave()
        {
            wave += 1;
        }

        public void StartWave()
        {
            SpawnEnemy();
        }
    }   
}
