using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;

    private Animator animator;

    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.down;

    private bool isAttacking = false;
    private bool isStunned = false;
    private float lastStunTime;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Stop player movement when stunned.
        if (isStunned)
        {
            movement = Vector2.zero;

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }

            return;
        }

        // Always run.
        ReadInput();
        MovePlayer();
        UpdateAnimation();
    }

    // Get player input and move Player accordingly.
    private void ReadInput()
    {
        movement = Vector2.zero;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            movement.x += 1;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            movement.x -= 1;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            movement.y += 1;

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            movement.y -= 1;

        if (movement.sqrMagnitude > 1f)
            movement.Normalize();
    }

    // Adjust player movement to align with game time.
    private void MovePlayer()
    {
        transform.Translate(movement * speed * Time.deltaTime);
    }

    // Update animation based on movement and triggers.
    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        // Play directional walking anim.
        if (!isAttacking && !isStunned)
        {
            if (movement.sqrMagnitude > 0.001f)
            {
                if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                    lastMoveDirection = new Vector2(Mathf.Sign(movement.x), 0f);
                else
                    lastMoveDirection = new Vector2(0f, Mathf.Sign(movement.y));

                animator.SetFloat("MoveX", lastMoveDirection.x);
                animator.SetFloat("MoveY", lastMoveDirection.y);
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }

    // Handle collisions.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit: " + collision.gameObject.name);

        // Destory EnemyTypeOne when hit.
        if (collision.gameObject.CompareTag("EnemyTypeOne"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(Attack());
        }

        // Have walls push back with same movement so Player does not go through them.
        if (collision.gameObject.CompareTag("Walls"))
        {
            transform.Translate(-movement * speed * Time.deltaTime);
        }
    }

    // Handle Player attacking EnemyTypeOne.
    private IEnumerator Attack()
    {
        isAttacking = true;

        // Update variables for directional animation.
        if (animator != null)
        {
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
            animator.SetTrigger("Attack");
        }

        // Make sure full animation can play.
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    // Handle player getting stunned when hit.
    public void Stun(float duration)
    {
        // Make sure Player does not get stun locked.
        if (Time.time - lastStunTime < 1f)
            return;

        
        lastStunTime = Time.time; // Set new lastStunTime.
        StartCoroutine(StunCoroutine(duration)); // Run stun animation.
    }

    // Run stun animation and stop movement.
    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        movement = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("Stunned", true);
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;

        if (animator != null)
        {
            animator.SetBool("Stunned", false);
        }
    }
}