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
        [SerializeField] private WaveUIManager waveUIManager;
        [SerializeField] private SaintsDictionary<int, SaintsDictionary<Enemy, int>> waveToEnemySpawner;
        [SerializeField] private SaintsDictionary<Enemy, int> killCounter = new SaintsDictionary<Enemy, int>();
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
            StartWave();
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

        void SpawnEnemy()
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
                        if (killCounter.ContainsKey(allEnemyDataForThisWave[i]))
                        {
                            killCounter[allEnemyDataForThisWave[i]] += 1;
                        }
                        else
                        {
                            killCounter[allEnemyDataForThisWave[i]] = 1;
                        }
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

        public void CheckIfWaveCleared(Component component, object enemyData)
        {
            Enemy enemy = (Enemy)component;
            if (!killCounter.ContainsKey(enemy))
            {
                return;
            }
            killCounter[enemy] -= 1;
            if (killCounter[enemy] == 0)
            {
                killCounter.Remove(enemy);
            }

            if (killCounter.Count == 0)
            {
                wave += 1;
                StartWave();
            }
        }

        public void StartWave()
        {
            waveUIManager.StartCountdown(() => SpawnEnemy());
        }
    }   
}
