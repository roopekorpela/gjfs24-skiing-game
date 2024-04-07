using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public GameObject[] pickups;

    public float spawnRange = 8f;
    public Transform playerTrans;
    public float offsetFromPlayer = 10f;

    public float pickupSpawnRate = 1f;
    public float obstacleSpawnRate = 5f;
    public float maxObstacleSpawnRate = 15f;
    public float distanceForRateIncrease = 100f;
    public float rateIncreaseAmount = 1f;

    private float nextObstacleSpawnYPosition;
    private float nextPickupSpawnYPosition;
    private float distanceTraveledSinceLastIncrease;
    private float lastYPosition;

    public List<GameObject> spawnedObjects;
    public float distanceThreshold = 20.0f;

    void Start()
    {
        lastYPosition = playerTrans.position.y;
        nextObstacleSpawnYPosition = playerTrans.position.y - offsetFromPlayer;
        nextPickupSpawnYPosition = playerTrans.position.y - offsetFromPlayer;
        StartCoroutine(DeleteOldObjectsRoutine());
    }

    void Update()
    {
        float distanceMoved = lastYPosition - playerTrans.position.y;
        if (distanceMoved > 0) 
        {
            distanceTraveledSinceLastIncrease += distanceMoved;
        }
        
        lastYPosition = playerTrans.position.y;

        //Increase rate:
        if (distanceTraveledSinceLastIncrease >= distanceForRateIncrease)
        {
            IncreaseSpawnRate();
            distanceTraveledSinceLastIncrease = 0f;
        }

        //OBstacles:
        if (playerTrans.position.y <= nextObstacleSpawnYPosition)
        {
            SpawnObstacle();
            nextObstacleSpawnYPosition -= offsetFromPlayer / obstacleSpawnRate;
        }

        //Pickups:
        if(playerTrans.position.y <= nextPickupSpawnYPosition)
        {
            SpawnPickup();
            nextPickupSpawnYPosition -= offsetFromPlayer / pickupSpawnRate;
        }
    }

    IEnumerator DeleteOldObjectsRoutine()
    {
        while (true)
        {
            DeleteOldObjects();
            yield return new WaitForSeconds(10);
        }
    }

    void DeleteOldObjects()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        foreach (var obj in spawnedObjects)
        {
            if (Vector3.Distance(playerTrans.position, obj.transform.position) > distanceThreshold)
            {
                objectsToDelete.Add(obj);
            }
        }

        foreach (var obj in objectsToDelete)
        {
            spawnedObjects.Remove(obj);
            Destroy(obj);
        }
    }

    void IncreaseSpawnRate()
    {
        obstacleSpawnRate = Mathf.Min(obstacleSpawnRate + rateIncreaseAmount, maxObstacleSpawnRate);
    }

    void SpawnObstacle()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), playerTrans.position.y - offsetFromPlayer, 0f);
        GameObject obs = Instantiate(obstacles[Random.Range(0, obstacles.Length)], spawnPosition, Quaternion.identity);
        spawnedObjects.Add(obs);
    }

    void SpawnPickup()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), playerTrans.position.y - offsetFromPlayer, 0f);
        GameObject pickup = Instantiate(pickups[Random.Range(0, pickups.Length)], spawnPosition, Quaternion.identity);
        spawnedObjects.Add(pickup);
    }

}
