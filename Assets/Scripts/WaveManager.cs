using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    
    [SerializeField] private int Waves;
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private List<int> enemiesPerWave;
    
    private List<SpawnManager> spawnManagers;
    private bool waveStarted;
    private int currentWave;

    public int enemiesAlive;
    
    
    void Start()
    {
        instance = this;
        spawnManagers = new List<SpawnManager>();
        foreach (GameObject spawnManager in GameObject.FindGameObjectsWithTag("Spawner")) 
        {
            var manager = spawnManager.GetComponent<SpawnManager>();
            if (manager != null) 
            {
                spawnManagers.Add(manager);
            } 
            else 
            {
                Debug.LogWarning($"Spawner {spawnManager.name} is missing a SpawnManager component.");
            }
        }
        StartCoroutine(StartNextWave());
    }

    private void Update()
    {
        if (!waveStarted && spawnManagers.TrueForAll(manager => manager !=null && manager.spawning))
        {
            Debug.Log("Spawning true");
            waveStarted = true;
            currentWave++;
        }
        
        waveCountText.text = currentWave.ToString();
    }

    public void EnemiesDied()
    {
        enemiesAlive--;
        gameManager.instance.updateGameGoal(-1);
        if (enemiesAlive <= 0 && waveStarted)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        waveStarted = false;
        
        if (currentWave < Waves)
        {
            StartCoroutine(StartNextWave());
        }
        else
        {
            gameManager.instance.youWin();
        }
    }

    IEnumerator StartNextWave()
    {
        int enemyCount = enemiesPerWave[currentWave];
        yield return new WaitForSeconds(3f);
        foreach (var manager in spawnManagers)
            manager.StartSpawning(enemyCount);
    }

  
}
