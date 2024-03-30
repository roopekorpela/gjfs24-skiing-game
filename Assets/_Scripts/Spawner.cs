using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public GameObject[] pickups;

    public float pickupSpawnInterval = 4f;
    public float obstacleSpawnInterval = 0.5f;
    public float spawnRange = 8f;
    public Transform playerTrans;
    public float offsetFromPlayer;

    void Start()
    {
        StartCoroutine(SpawnObstacles());
        StartCoroutine(SpawnPickups());
    }

    IEnumerator SpawnObstacles()
    {
        while (true)
        {
            yield return new WaitForSeconds(obstacleSpawnInterval);
            SpawnObstacle();
        }
    }

    IEnumerator SpawnPickups()
    {
        while (true)
        {
            yield return new WaitForSeconds(pickupSpawnInterval);
            SpawnPickup();
        }
    }
    void SpawnPickup()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), playerTrans.position.y + offsetFromPlayer, 0f);
        Instantiate(pickups[Random.Range(0, pickups.Length)], spawnPosition, Quaternion.identity);
    }

    void SpawnObstacle()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), playerTrans.position.y + offsetFromPlayer, 0f);
        Instantiate(obstacles[Random.Range(0, obstacles.Length)], spawnPosition, Quaternion.identity);
    }
}
