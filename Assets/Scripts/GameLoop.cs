using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;

    // Prefabs
    public GameObject enemyTypeOnePrefab;
    public GameObject enemyTypeTwoPrefab;
    public GameObject enemyTypeThreePrefab;

    // TypeTwo: always keep this many alive
    [SerializeField] private int typeTwoCount = 2;
    private List<GameObject> currentTypeTwos = new List<GameObject>();

    // TypeThree: keep one alive
    private GameObject currentTypeThree;

    // TypeOne spawn settings
    [SerializeField] private float typeOneSpawnInterval = 5f;
    [SerializeField] private float difficultyIncreaseInterval = 10f;
    [SerializeField] private float spawnRateMultiplier = 0.99f; // 1% faster every 10 seconds
    [SerializeField] private float minSpawnInterval = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    // Make sure all instances needed are created.
    private void Start()
    {
        if (enemyTypeOnePrefab == null)
            Debug.LogError("GameLoop: enemyTypeOnePrefab not assigned.");

        if (enemyTypeTwoPrefab == null)
            Debug.LogError("GameLoop: enemyTypeTwoPrefab not assigned.");

        if (enemyTypeThreePrefab == null)
            Debug.LogError("GameLoop: enemyTypeThreePrefab not assigned.");

        if (enemyTypeTwoPrefab != null)
        {
            for (int i = 0; i < typeTwoCount; i++)
            {
                SpawnTypeTwo();
            }
        }

        if (enemyTypeThreePrefab != null)
        {
            SpawnTypeThree();
        }

        if (enemyTypeOnePrefab != null)
        {
            StartCoroutine(SpawnTypeOneLoop());
        }

        StartCoroutine(IncreaseDifficulty());
    }

    // Check to make sure ETT and ETH are all present.
    private void Update()
    {
        currentTypeTwos.RemoveAll(enemy => enemy == null);

        while (currentTypeTwos.Count < typeTwoCount && enemyTypeTwoPrefab != null)
        {
            SpawnTypeTwo();
        }

        if (currentTypeThree == null && enemyTypeThreePrefab != null)
        {
            SpawnTypeThree();
        }
    }

    // Constantly spawn a new ETO on every spawn interval.
    private IEnumerator SpawnTypeOneLoop()
    {
        while (true)
        {
            SpawnTypeOne();
            yield return new WaitForSeconds(typeOneSpawnInterval);
        }
    }

    // Increase spawn rate as time goes on.
    private IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            typeOneSpawnInterval = Mathf.Max(
                minSpawnInterval,
                typeOneSpawnInterval * spawnRateMultiplier
            );

            Debug.Log("TypeOne spawn interval is now: " + typeOneSpawnInterval);
        }
    }

    // Spawn a new instance of ETO.
    private void SpawnTypeOne()
    {
        if (enemyTypeOnePrefab == null)
            return;

        Instantiate(enemyTypeOnePrefab);
    }

    // Spawn a new instance of ETT. 
    private void SpawnTypeTwo()
    {
        if (enemyTypeTwoPrefab == null)
            return;

        GameObject newEnemy = Instantiate(enemyTypeTwoPrefab);
        currentTypeTwos.Add(newEnemy);
    }

    // Spawn a new instance of ETH.
    private void SpawnTypeThree()
    {
        if (enemyTypeThreePrefab == null)
            return;

        currentTypeThree = Instantiate(enemyTypeThreePrefab);
    }
}