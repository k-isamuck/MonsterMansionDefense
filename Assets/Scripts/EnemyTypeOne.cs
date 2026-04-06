using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeOne : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    [SerializeField] private float moveSpeed = 0.01f;

    private void Start()
    {
        if (AStarManager.instance == null)
        {
            Debug.LogError("AStarManager.instance is null.");
            return;
        }

        Node[] nodes = FindObjectsOfType<Node>();

        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogError("No nodes found in scene.");
            return;
        }

        Node centerNode = null;

        foreach (Node node in nodes)
        {
            if (node.gameObject.name == "Center")
            {
                centerNode = node;
                break;
            }
        }

        if (centerNode == null)
        {
            Debug.LogError("No node named 'Center' was found.");
            return;
        }

        List<Node> spawnableNodes = new List<Node>();

        foreach (Node node in nodes)
        {
            if (node != null && node != centerNode)
            {
                spawnableNodes.Add(node);
            }
        }

        if (spawnableNodes.Count == 0)
        {
            Debug.LogError("No valid spawn nodes found.");
            return;
        }

        currentNode = spawnableNodes[Random.Range(0, spawnableNodes.Count)];

        transform.position = new Vector3(
            currentNode.transform.position.x,
            currentNode.transform.position.y,
            1f
        );

        path = AStarManager.instance.GeneratePath(currentNode, centerNode);

        if (path == null)
        {
            path = new List<Node>();
            Debug.LogError("No path could be generated to Center.");
            return;
        }

        if (path.Count > 0 && path[0] == currentNode)
        {
            path.RemoveAt(0);
        }
    }

    private void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (path == null || path.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        Node targetNode = path[0];

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(targetNode.transform.position.x, targetNode.transform.position.y, 1f),
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetNode.transform.position) < 0.1f)
        {
            currentNode = targetNode;
            path.RemoveAt(0);

            if (path.Count == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}