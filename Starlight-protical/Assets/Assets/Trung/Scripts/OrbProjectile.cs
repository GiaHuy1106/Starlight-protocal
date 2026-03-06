using UnityEngine;

public class OrbProjectile : MonoBehaviour
{   
    public GameObject explosionPrefab;
    public float speed = 10f;
    public int damage; // damage nhận từ player
    public float maxDistance = 10f;
    public LayerMask enemyLayer;
    Rigidbody rb;
    Vector3 startPos;
    Vector3 direction;
    Transform target;
    bool exploded = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        startPos = transform.position;
        Collider playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
        Collider orbCol = GetComponent<Collider>();

        Physics.IgnoreCollision(playerCol, orbCol);
        Destroy(gameObject, 5f);
    }   
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.forward = direction;
        rb.linearVelocity = direction * speed; 
    }
        public void SetTarget(Transform enemy)
        {
            target = enemy;
        }
    void Update()
    {if (exploded) return;

        // nếu có target → tracking
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;

            rb.linearVelocity = dir * speed;

            transform.forward = dir;
        }
        // nếu bay quá xa → nổ
        if (!exploded && Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Explode();
        }
    }
    void OnTriggerEnter(Collider other)
    {
         Debug.Log("Hit: " + other.name);
        if (exploded) return;

        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Enemy1 enemy = other.GetComponent<Enemy1>();
            if (enemy != null)
            {
                float finalDamage = damage * 100f / (100f + enemy.def); // công thức tính damage có phòng thủ của enemy
                enemy.TakeDamage((int)finalDamage);
            }
            Explode();
        }
    }
    void Explode()
    {
        if (exploded) return;
        exploded = true;
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
