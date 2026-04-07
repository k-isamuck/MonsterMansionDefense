using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;

    // Prefabs.
    public GameObject enemyTypeOnePrefab;
    public GameObject enemyTypeTwoPrefab;
    public GameObject enemyTypeThreePrefab;

    // Only one instance of TypeTwo and TypeThree enemies at a time.
    private GameObject currentTypeTwo;
    private GameObject currentTypeThree;

    // TypeOne spawnrate.
    [SerializeField] private float typeOneSpawnInterval = 5f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Check that all PreFabs are established.
        if (enemyTypeOnePrefab == null)
            Debug.LogError("GameLoop: enemyTypeOnePrefab not assigned.");

        if (enemyTypeTwoPrefab == null)
            Debug.LogError("GameLoop: enemyTypeTwoPrefab not assigned.");

        if (enemyTypeThreePrefab == null)
            Debug.LogError("GameLoop: enemyTypeThreePrefab not assigned.");

        // Spawn all enemies at the start.
        if (enemyTypeTwoPrefab != null)
            SpawnTypeTwo();

        if (enemyTypeThreePrefab != null)
            SpawnTypeThree();

        if (enemyTypeOnePrefab != null)
            StartCoroutine(SpawnTypeOneLoop());
    }

    private void Update()
    {   
        // Type two and three enemies should not be deleted. Spawn new instance in the case that they are.
        if (currentTypeTwo == null && enemyTypeTwoPrefab != null)
        {
            SpawnTypeTwo();
        }

        if (currentTypeThree == null && enemyTypeThreePrefab != null)
        {
            SpawnTypeThree();
        }
    }

    // Continue to spawn TypeOne Enemies on each interval.
    private IEnumerator SpawnTypeOneLoop()
    {
        while (true)
        {
            SpawnTypeOne();
            yield return new WaitForSeconds(typeOneSpawnInterval);
        }
    }

    // Instantiate new TypeOne enemy.
    private void SpawnTypeOne()
    {
        if (enemyTypeOnePrefab == null)
            return;

        Instantiate(enemyTypeOnePrefab);
    }

    // Instantiate new TypeTwo enemy.
    private void SpawnTypeTwo()
    {
        if (enemyTypeTwoPrefab == null)
            return;

        currentTypeTwo = Instantiate(enemyTypeTwoPrefab);
    }

    // Instantiate new TypeThree enemy.
    private void SpawnTypeThree()
    {
        if (enemyTypeThreePrefab == null)
            return;

        currentTypeThree = Instantiate(enemyTypeThreePrefab);
    }
}