using System.Collections.Generic;
using System.Linq;
using Events;
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
        [SerializeField] private SaintsDictionary<string, int> killCounter = new SaintsDictionary<string, int>();
        [SerializeField] private RegionBorder.RegionBorder initialMap;
        [SerializeField] private RegionBorder.RegionBorder currentMap;
        private RegionBorder.RegionBorder previousMap;
        [SerializeField] private float minX;
        [SerializeField] private float minY;
        [SerializeField] private float maxX;
        [SerializeField] private float maxY;
        [SerializeField] private CrossObjectEventWithDataSO setNewRandomTargetEnemy;
        [SerializeField] private CrossObjectEventWithDataSO setNewRandomTargetBoss;
        private List<Enemy> allSpawnedEnemies = new List<Enemy>();
        private List<Boss> allSpawnedBoss = new List<Boss>();
        
        void Start()
        {
            SetBoundary(initialMap);
            previousMap = currentMap;
            currentMap = initialMap;
            StartWave();
        }

        public int GetWave()
        {
            return wave + 1;
        }

        public void SetCurrentRegion(RegionBorder.RegionBorder regionBorder)
        {
            previousMap = currentMap;
            currentMap = regionBorder;
            
            //There is a very weird bug where crashing into a locked map
            //causes the current map to be that locked map. This fixes
            //the spawning issue
            if (!currentMap.GetMapIsUnlocked())
            {
                SetBoundary(previousMap);
            }
            else
            {
                SetBoundary(currentMap);
            }
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
            Vector2 topLeft = new Vector2(minX + 3, maxY - 3);
            Vector2 bottomLeft = new Vector2(minX + 3, minY + 3);
            Vector2 topRight = new Vector2(maxX - 3, maxY - 3);
            Vector2 bottomRight = new Vector2(maxX - 3, minY + 3);
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
                        if (killCounter.ContainsKey(allEnemyDataForThisWave[i].getEnemyId()))
                        {
                            killCounter[allEnemyDataForThisWave[i].getEnemyId()] += 1;
                        }
                        else
                        {
                            killCounter[allEnemyDataForThisWave[i].getEnemyId()] = 1;
                        }
                        isEnemyStillSpawning = true;
                        Enemy spawnedEnemy = Instantiate(allEnemyDataForThisWave[i], spawnPoints[spawnIndex], Quaternion.identity);
                        if (spawnedEnemy.GetComponent<Boss>() != null) 
                        {
                            allSpawnedBoss.Add((Boss) spawnedEnemy);
                        }
                        else
                        {
                            allSpawnedEnemies.Add(spawnedEnemy);
                        }
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
            GetNewTargetEnemy();
            GetNewTargetBoss();
        }

        public void CheckIfWaveCleared(Component component, object enemyData)
        {
            if (component is Enemy)
            {
                Enemy enemy = (Enemy)component;
                if (!killCounter.ContainsKey(enemy.getEnemyId()))
                {
                    return;
                }
                allSpawnedEnemies.Remove(enemy);
                killCounter[enemy.getEnemyId()] -= 1;
                GetNewTargetEnemy();
                if (killCounter[enemy.getEnemyId()] == 0)
                {
                    killCounter.Remove(enemy.getEnemyId());
                }
            }
            else
            {
                Boss boss = (Boss) component;
                if (!killCounter.ContainsKey(boss.getEnemyId()))
                {
                    return;
                }
                allSpawnedBoss.Remove(boss);
                killCounter[boss.getEnemyId()] -= 1;
                GetNewTargetBoss();
                if (killCounter[boss.getEnemyId()] == 0)
                {
                    killCounter.Remove(boss.getEnemyId());
                }
            }

            if (killCounter.Count == 0)
            {
                wave += 1;
                StartWave();
            }
        }

        public void GetNewTargetEnemy()
        {
            if (allSpawnedEnemies.Count - 1 >= 0) setNewRandomTargetEnemy.TriggerEvent(allSpawnedEnemies[Random.Range(0, allSpawnedEnemies.Count - 1)]);
        }
        
        public void GetNewTargetBoss()
        {
            if (allSpawnedBoss.Count - 1 >= 0) setNewRandomTargetBoss.TriggerEvent(allSpawnedBoss[Random.Range(0, allSpawnedBoss.Count - 1)]);
        }

        public void StartWave()
        {
            waveUIManager.StartCountdown(() => SpawnEnemy());
        }

        public void ResetWave()
        {
            foreach (var enemy in allSpawnedEnemies)
            {
                Destroy(enemy.gameObject);
            }
            allSpawnedEnemies.Clear();
        }

        public void AddBossToNextWave(Component component, object enemy)
        {
            Boss enemyInformation = (Boss)component;
            if (waveToEnemySpawner[wave + 1].ContainsKey(enemyInformation))
            {
                waveToEnemySpawner[wave + 1][enemyInformation]++;
            }
            else
            {
                waveToEnemySpawner[wave + 1][enemyInformation] = 1;
            }
        }
    }   
}
