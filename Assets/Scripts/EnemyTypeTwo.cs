using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeTwo : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    [SerializeField] private float moveSpeed = 3f;

    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;

    private void Start()
    {
        animator = GetComponent<Animator>();

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
            Vector3 targetPosition = new Vector3(
                path[0].transform.position.x,
                path[0].transform.position.y,
                1
            );

            Vector2 moveDirection = (targetPosition - transform.position).normalized;

            UpdateAnimation(moveDirection);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, path[0].transform.position) < 0.1f)
            {
                currentNode = path[0];
                path.RemoveAt(0);
            }
        }
        else
        {
            UpdateAnimation(Vector2.zero);

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