using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
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
    [SerializeField] private int maxEnemies = 10;

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        timeSinceLastWave += Time.deltaTime;

        if (timeSinceLastWave >= waveRate)
        {
            timeSinceLastWave = 0f;
            waveNumber++;
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < numberPerWave; i++)
        {
            //if the spawn rate has passed and the number of children of this gameobject is less than the max number of enemies
            if (timeSinceLastSpawn >= spawnRate && enemiesSpawned < maxEnemies)
            {
                timeSinceLastSpawn = 0f;
                enemiesSpawned++;
                Instantiate(prefab, transform.position, Quaternion.LookRotation(firstWaypointPosition.position), transform);
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }
}