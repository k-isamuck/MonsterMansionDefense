using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        if (start == null)
        {
            Debug.LogError("GeneratePath: start is null");
            return null;
        }

        if (end == null)
        {
            Debug.LogError("GeneratePath: end is null");
            return null;
        }

        List<Node> nodesNeedToCheck = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
            n.hScore = 0;
            n.cameFrom = null;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        nodesNeedToCheck.Add(start);

        while (nodesNeedToCheck.Count > 0)
        {
            int lowestF = 0;

            for (int i = 1; i < nodesNeedToCheck.Count; i++)
            {
                if (nodesNeedToCheck[i].FScore() < nodesNeedToCheck[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = nodesNeedToCheck[lowestF];
            nodesNeedToCheck.RemoveAt(lowestF);

            if (currentNode == end)
            {
                List<Node> path = new List<Node>();
                Node pathNode = end;

                while (pathNode != null)
                {
                    path.Add(pathNode);

                    if (pathNode == start)
                        break;

                    pathNode = pathNode.cameFrom;
                }

                path.Reverse();
                return path;
            }

            if (currentNode.connections == null)
                continue;

            foreach (Node connectedNode in currentNode.connections)
            {
                if (connectedNode == null) continue;

                float tentativeGScore =
                    currentNode.gScore +
                    Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (tentativeGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = tentativeGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!nodesNeedToCheck.Contains(connectedNode))
                    {
                        nodesNeedToCheck.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }
}