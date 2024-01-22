using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefab;
    [SerializeField] float minRandomSpawn = -500f;
    [SerializeField] float maxRandomSpawn = 500f;
    [SerializeField] Color color;
    public void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float randomX = Random.Range(minRandomSpawn, maxRandomSpawn) + transform.position.x;
            float randomY = Random.Range(minRandomSpawn, maxRandomSpawn) + transform.position.y;
            float randomZ = Random.Range(minRandomSpawn, maxRandomSpawn) + transform.position.z;
            Vector3 randomSpawnPoint = new Vector3(randomX, randomY, randomZ);
            int randomPrefab = Random.Range(0, prefab.Length);
            Instantiate(prefab[randomPrefab], randomSpawnPoint, Random.rotation, this.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, new Vector3(maxRandomSpawn * 2, maxRandomSpawn * 2, maxRandomSpawn * 2));
    }
}
