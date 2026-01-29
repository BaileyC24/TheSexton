using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private float spawnBuffer;
    [SerializeField] private gameManager gameManager;

    public bool spawning;

    private void Start()
    {
        spawnPoints = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints.Add(transform.GetChild(i).gameObject);
        }

        
    }

    public void StartSpawning(int enemyCount)
    {
        StartCoroutine(Spawn(enemyCount));



    }
    
    IEnumerator Spawn(int enemyCount)
    {
        spawning = true;

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError($"{name} has no spawn points assigned!");
            yield break;
        }
        
        gameManager.updateGameGoal(enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject spawnPoint = spawnPoints[i % spawnPoints.Count];
            Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            WaveManager.instance.enemiesAlive++;
            yield return new WaitForSeconds(0.5f);
        }

        spawning = false;

    }

   // public void EnemyDied() //0125
   // {
    //    currentEnemyCount --; 
   // }
    
}
