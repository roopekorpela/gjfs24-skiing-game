using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject[] items;
    public float spawnInterval = 1f;
    public float pickupSpawnInterval = 5f;
    public float obstacleSpawnInterval = 3f;
    public float spawnRange = 8f;
    public Transform playerTrans;
    public float offSetFromPlayer;

    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(obstacleSpawnInterval);
            SpawnObstacle();
        }
    }
    void SpawnPickup()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), playerTrans.position.y + offSetFromPlayer, 0f);
    }

    void SpawnObstacle()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), playerTrans.position.y + offSetFromPlayer, 0f);
        Instantiate(items[Random.Range(0, items.Length)], spawnPosition, Quaternion.identity);
    }
}
