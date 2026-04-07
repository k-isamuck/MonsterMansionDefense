using UnityEngine;

public class EnemyTypeThreeProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float stunDuration = 0.5f;

    private Vector2 moveDirection = Vector2.right;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Walls"))
        {
            Destroy(gameObject);
            return;
        }

        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.Stun(stunDuration);
            Destroy(gameObject);
        }
    }
}