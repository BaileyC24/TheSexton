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
        foreach (var spawnPoint in spawnPoints)
        {
            Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            WaveManager.instance.enemiesAlive++;
            gameManager.updateGameGoal(1);
            yield return new WaitForSeconds(0.5f);
        }
        spawning = false;
    }
    
}
