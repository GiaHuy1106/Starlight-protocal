using UnityEngine;
// * Dùng để điều khiển viên đạn lửa bắn ra từ kỹ năng của người chơi

// * Viên đạn sẽ bay theo hướng đã được thiết lập và phát nổ khi chạm kẻ địch hoặc đạt khoảng cách tối đa
// * Khi phát nổ, nó sẽ tạo hiệu ứng nổ tại vị trí hiện tại và hủy bản thân
public class FireballProjectile : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float maxDistance = 10f;
    public float speed = 10f;
    public int damage; // damage nhận từ player
    bool exploded = false;
    Rigidbody rb;

    Vector3 startPos;
    Vector3 direction;

    // nhận direction từ PlayerSkill
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
        if (exploded) return;
        
        if (other.CompareTag("Player")) return;

        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
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
