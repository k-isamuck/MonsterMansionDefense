using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeThree : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float repathInterval = 0.5f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootInterval = 1.5f;
    [SerializeField] private float sightRange = 8f;
    [SerializeField] private LayerMask sightBlockLayers;

    private Transform player;
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool isAttacking = false;
    private float shootTimer;
    private float repathTimer;

    private void Start()
    {
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("EnemyTypeThree: Player not found.");
            return;
        }

        player = playerObject.transform;

        if (currentNode == null)
        {
            currentNode = GetClosestNodeToPosition(transform.position);
        }

        if (animator != null)
        {
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsShooting", false);
        }

        RecalculatePathToPlayer();
    }

    private void Update()
    {
        if (player == null || AStarManager.instance == null)
            return;

        if (isAttacking)
            return;

        shootTimer += Time.deltaTime;
        repathTimer += Time.deltaTime;

        if (CanSeePlayer() && shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            StartCoroutine(AttackAndShoot());
            return;
        }

        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            RecalculatePathToPlayer();
        }

        MoveAlongPath();
    }

    private void RecalculatePathToPlayer()
    {
        if (currentNode == null)
        {
            currentNode = GetClosestNodeToPosition(transform.position);
        }

        Node playerClosestNode = GetClosestNodeToPosition(player.position);

        if (currentNode == null || playerClosestNode == null)
            return;

        path = AStarManager.instance.GeneratePath(currentNode, playerClosestNode);

        if (path == null)
        {
            path = new List<Node>();
            return;
        }

        if (path.Count > 0 && path[0] == currentNode)
        {
            path.RemoveAt(0);
        }
    }

    private void MoveAlongPath()
    {
        if (path == null || path.Count == 0)
        {
            UpdateAnimation(Vector2.zero);
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
        }
    }

    private Node GetClosestNodeToPosition(Vector3 position)
    {
        Node[] nodes = FindObjectsOfType<Node>();

        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogError("EnemyTypeThree: No nodes found.");
            return null;
        }

        float closestDistance = Mathf.Infinity;
        Node closestNode = null;

        foreach (Node node in nodes)
        {
            if (node == null)
                continue;

            float dist = Vector2.Distance(position, node.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestNode = node;
            }
        }

        return closestNode;
    }

    private void UpdateAnimation(Vector2 moveDirection)
    {
        if (animator == null)
            return;

        if (isAttacking)
            return;

        bool isMoving = moveDirection.sqrMagnitude > 0.001f;

        if (isMoving)
        {
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
            {
                lastMoveDirection = new Vector2(Mathf.Sign(moveDirection.x), 0f);
            }
            else
            {
                lastMoveDirection = new Vector2(0f, Mathf.Sign(moveDirection.y));
            }
        }

        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);
        animator.SetBool("IsMoving", isMoving);
    }

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > sightRange)
            return false;

        RaycastHit2D hit;

        if (sightBlockLayers.value == 0)
        {
            hit = Physics2D.Raycast(transform.position, toPlayer.normalized, sightRange);
        }
        else
        {
            hit = Physics2D.Raycast(transform.position, toPlayer.normalized, sightRange, sightBlockLayers);
        }

        if (hit.collider == null)
            return false;

        return hit.collider.CompareTag("Player");
    }

    private IEnumerator AttackAndShoot()
    {
        if (isAttacking)
            yield break;

        isAttacking = true;

        Vector2 dir = (player.position - transform.position).normalized;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            lastMoveDirection = new Vector2(Mathf.Sign(dir.x), 0f);
        }
        else
        {
            lastMoveDirection = new Vector2(0f, Mathf.Sign(dir.y));
        }

        if (animator != null)
        {
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsShooting", true);
        }

        ShootAtPlayer();

        yield return new WaitForSeconds(attackDuration);

        if (animator != null)
        {
            animator.SetBool("IsShooting", false);
        }

        isAttacking = false;
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("EnemyTypeThree: projectilePrefab not assigned.");
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        Vector2 direction = (player.position - spawnPoint.position).normalized;

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        EnemyTypeThreeProjectile projectile = projectileObject.GetComponent<EnemyTypeThreeProjectile>();
        if (projectile != null)
        {
            projectile.SetDirection(direction);
        }
    }
}