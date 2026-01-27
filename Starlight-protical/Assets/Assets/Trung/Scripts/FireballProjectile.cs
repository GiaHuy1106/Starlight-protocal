using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public GameObject explosionPrefab;

    public float maxDistance = 10f;
    public float speed = 10f;
    bool exploded = false;
    Rigidbody rb;

    Vector3 startPos;
    Vector3 direction;

    // ⭐ nhận direction từ PlayerSkill
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        // xoay visual cho đẹp (không ảnh hưởng vật lý)
        transform.forward = direction;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startPos = transform.position;

        rb.linearVelocity = direction * speed;

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (!exploded && Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!exploded && other.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
