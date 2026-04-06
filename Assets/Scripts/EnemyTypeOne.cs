using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeOne : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    [SerializeField] private float moveSpeed = 0.01f;

    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;

    private void Start()
    {
        animator = GetComponent<Animator>();

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

        HashSet<string> blockedNodes = new HashSet<string>()
        {
            "Node8I", "Node8L",
            "Node9H", "Node9I", "Node9J", "Node9K", "Node9L", "Node9M",
            "Node10I", "Node10L",
            "Node11I", "Node11L",
            "Node12I", "Node12J", "Node12K", "Node12L",
            "Node13I", "Node13L",
            "Center"
        };

        foreach (Node node in nodes)
        {
            if (node != null && !blockedNodes.Contains(node.gameObject.name))
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
            UpdateAnimation(Vector2.zero);
            Destroy(gameObject);
            return;
        }

        Node targetNode = path[0];

        Vector3 targetPosition = new Vector3(
            targetNode.transform.position.x,
            targetNode.transform.position.y,
            1f
        );

        Vector2 moveDirection = (targetPosition - transform.position).normalized;
        UpdateAnimation(moveDirection);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetNode.transform.position) < 0.1f)
        {
            currentNode = targetNode;
            path.RemoveAt(0);

            if (path.Count == 0)
            {
                UpdateAnimation(Vector2.zero);
                Destroy(gameObject);
            }
        }
    }

    private void UpdateAnimation(Vector2 moveDirection)
    {
        if (animator == null)
            return;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
            {
                lastMoveDirection = new Vector2(Mathf.Sign(moveDirection.x), 0f);
            }
            else
            {
                lastMoveDirection = new Vector2(0f, Mathf.Sign(moveDirection.y));
            }

            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
            animator.SetBool("IsMoving", false);
        }
    }
}