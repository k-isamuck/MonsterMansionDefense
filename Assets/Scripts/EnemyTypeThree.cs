using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeThree : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackDuration = 0.5f;

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

        if (animator != null)
        {
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
        }
    }

    private void Update()
    {
        if (isAttacking)
            return;

        shootTimer += Time.deltaTime;

        if (player != null && CanSeePlayer() && shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            StartCoroutine(AttackAndShoot());
            return;
        }

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

        if (isAttacking)
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
        }

        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);
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

        Vector2 dir = (player.transform.position - transform.position).normalized;

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
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        ShootAtPlayer();

        yield return new WaitForSeconds(attackDuration);

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