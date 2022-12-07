using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private float waveRate = 5f;
    [SerializeField] private int numberPerWave = 5;

    private float timeSinceLastSpawn = 0f;

    //spawn a wave of enemies every waveRate seconds
    private float timeSinceLastWave = 0f;

    private int waveNumber = 0;

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
            if (timeSinceLastSpawn >= spawnRate)
            {
                timeSinceLastSpawn = 0f;
                Instantiate(prefab, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }
}