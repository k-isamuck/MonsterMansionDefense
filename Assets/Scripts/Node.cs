using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections = new List<Node>();

    public float gScore;
    public float hScore;

    public float FScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (connections == null) return;

        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i] != null)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }
}