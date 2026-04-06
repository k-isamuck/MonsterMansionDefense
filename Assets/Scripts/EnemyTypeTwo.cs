using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeTwo : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    private void Start()
    {
        if (currentNode == null)
        {
            Node[] nodes = FindObjectsOfType<Node>();
            float closestDistance = Mathf.Infinity;
            Node closestNode = null;

            foreach (Node node in nodes)
            {
                float dist = Vector2.Distance(transform.position, node.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestNode = node;
                }
            }

            currentNode = closestNode;
        }
    }

    private void Update()
    {
        CreatePath();
    }

    public void CreatePath()
    {
        if (path != null && path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(path[0].transform.position.x, path[0].transform.position.y, 1),
                3 * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, path[0].transform.position) < 0.1f)
            {
                currentNode = path[0];
                path.RemoveAt(0);
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();

            if (AStarManager.instance == null)
            {
                Debug.LogError("AStarManager.instance is null");
                return;
            }

            if (currentNode == null)
            {
                Debug.LogError("currentNode is null");
                return;
            }

            if (nodes.Length == 0)
            {
                Debug.LogError("No nodes found");
                return;
            }

            Node target = nodes[Random.Range(0, nodes.Length)];
            path = AStarManager.instance.GeneratePath(currentNode, target);

            if (path == null)
            {
                path = new List<Node>();
            }
        }
    }
}