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
    
    
    private List<SpawnManager> spawnManagers;
    private bool waveStarted;
    private int currentWave;
    
    void Start()
    {
        instance = this;
        spawnManagers = new List<SpawnManager>();

        foreach (GameObject spawnManager in GameObject.FindGameObjectsWithTag("Spawner"))
            spawnManagers.Add(spawnManager.GetComponent<SpawnManager>());
    }

    private void Update()
    {
        if (!waveStarted && spawnManagers.TrueForAll(manager => manager.spawning))
        {
            Debug.Log("Spawning true");
            waveStarted = true;
            currentWave++;
        }
        
        waveCountText.text = currentWave.ToString();
    }
}
