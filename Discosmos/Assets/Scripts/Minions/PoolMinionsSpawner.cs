using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PoolMinionsSpawner : MonoBehaviour
{
    [SerializeField] private Transform firstWaypointPosition;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private float waveRate = 5f;
    [SerializeField] private int numberPerWave = 5;

    private float timeSinceLastSpawn = 0f;

    //spawn a wave of enemies every waveRate seconds
    private float timeSinceLastWave = 0f;

    private int waveNumber = 0;
    private int enemiesSpawned = 0;
    [SerializeField] private int poolSize = 10;
    
    //create a pool of enemies
    private List<GameObject> pool = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            //create the pool of enemies
            for (int i = 0; i < poolSize; i++)
            {
                GameObject enemy = PhotonNetwork.Instantiate(prefab.name, firstWaypointPosition.position, Quaternion.identity);
                enemy.SetActive(false);
                pool.Add(enemy);
            }   
        }
        else
        {
            this.enabled = false;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        timeSinceLastWave += Time.deltaTime;
        
        //spawn a wave of enemies every waveRate seconds
        if (timeSinceLastWave >= waveRate)
        {
            timeSinceLastWave = 0f;
            waveNumber++;
            enemiesSpawned = 0;
        }
        
        //spawn enemies
        if (timeSinceLastSpawn >= spawnRate && enemiesSpawned < numberPerWave)
        {
            timeSinceLastSpawn = 0f;
            enemiesSpawned++;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        
        //find an inactive enemy in the pool
        foreach (GameObject enemy in pool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.transform.position = transform.position;
                enemy.SetActive(true);
                return;
            }
        }
    }
}
