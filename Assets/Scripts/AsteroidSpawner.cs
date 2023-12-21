using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] asteroidPrefab;
    [SerializeField] int asteroidAmount = 200;
    [SerializeField] float minRandomSpawn = -500f;
    [SerializeField] float maxRandomSpawn = 500f;
    void Start()
    {
        SpawnAsteroid();
    }

    private void SpawnAsteroid()
    {
        for (int i = 0; i < asteroidAmount; i++)
        {
            float randomScale = Random.Range(0.5f, 4f);
            float randomX = Random.Range(minRandomSpawn, maxRandomSpawn);
            float randomY = Random.Range(minRandomSpawn, maxRandomSpawn);
            float randomZ = Random.Range(minRandomSpawn, maxRandomSpawn);
            Vector3 randomSpawnPoint = new Vector3(randomX, randomY, randomZ);
            int randomPrefab = Random.Range(0, asteroidPrefab.Length);
            GameObject tmp = Instantiate(asteroidPrefab[randomPrefab], randomSpawnPoint, Random.rotation,this.transform);
            tmp.transform.localScale = Vector3.one * randomScale;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(maxRandomSpawn * 2, maxRandomSpawn * 2, maxRandomSpawn * 2));
    }
}
