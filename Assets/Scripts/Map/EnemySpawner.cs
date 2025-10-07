using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner1 : MonoBehaviour
{
    [field: SerializeField] private GameObject enemyPrefab;
    [field: SerializeField] private float enemyInterval = 3.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawnEnemy(enemyInterval, enemyPrefab));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            Vector3 spawnPos = new Vector3(Random.Range(-5f, 5), Random.Range(-5f, 5), 0);
            Instantiate(enemy, spawnPos, Quaternion.identity);
        }    
        
    }
}
