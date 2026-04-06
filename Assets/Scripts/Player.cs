using UnityEngine;

public class Player : MonoBehaviour
{
    float speed = 4.0f;

    private Animator animator;
    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isAttacking)
            return; 

        movement = Vector2.zero;

        // Input
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movement.x += 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movement.x -= 1;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            movement.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            movement.y -= 1;
        }

        // Normalize diagonal movement
        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        // Move
        transform.Translate(movement * speed * Time.deltaTime);

        // Animate
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator == null)
            return;

        if (movement.sqrMagnitude > 0.001f)
        {
            // Snap to 4 directions
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                lastMoveDirection = new Vector2(Mathf.Sign(movement.x), 0f);
            }
            else
            {
                lastMoveDirection = new Vector2(0f, Mathf.Sign(movement.y));
            }

            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            // Idle (single animation)
            animator.SetBool("IsMoving", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("EnemyTypeOne"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(Attack());
        }
        
        if (collision.gameObject.tag == "Walls")
        {
            transform.Translate(-movement * speed * Time.deltaTime);
        }
        
    }

    System.Collections.IEnumerator Attack()
    {
        isAttacking = true;

        // Make sure correct direction is used
        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f); // match animation length

        isAttacking = false;
    }
}