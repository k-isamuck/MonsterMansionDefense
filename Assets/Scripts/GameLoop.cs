using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;

    public GameObject enemyTypeOnePrefab;
    public GameObject enemyTypeTwoPrefab;

    private GameObject currentTypeOne;
    private List<GameObject> currentTypeTwos = new List<GameObject>();

    [SerializeField] private int typeTwoCount = 2;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (enemyTypeOnePrefab == null)
            Debug.LogError("GameLoop: enemyTypeOnePrefab is not assigned.");

        if (enemyTypeTwoPrefab == null)
            Debug.LogError("GameLoop: enemyTypeTwoPrefab is not assigned.");

        SpawnTypeOne();

        for (int i = 0; i < typeTwoCount; i++)
        {
            SpawnTypeTwo();
        }
    }

    private void Update()
    {
        if (currentTypeOne == null && enemyTypeOnePrefab != null)
        {
            SpawnTypeOne();
        }

        currentTypeTwos.RemoveAll(enemy => enemy == null);

        while (currentTypeTwos.Count < typeTwoCount && enemyTypeTwoPrefab != null)
        {
            SpawnTypeTwo();
        }
    }

    private void SpawnTypeOne()
    {
        if (enemyTypeOnePrefab == null) return;

        currentTypeOne = Instantiate(enemyTypeOnePrefab);
    }

    private void SpawnTypeTwo()
    {
        if (enemyTypeTwoPrefab == null) return;

        GameObject newEnemy = Instantiate(enemyTypeTwoPrefab);
        currentTypeTwos.Add(newEnemy);
    }
}