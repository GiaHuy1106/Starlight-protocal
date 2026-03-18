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
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    bool exploded = false;
    
    Rigidbody rb;

    Vector3 startPos;
    Vector3 direction;
    GameObject attacker;

    // Âm thanh nổ giống OrbProjectile
    public AudioClip explosionSFX;
    [Range(0f, 1f)]
    public float explosionVolume = 1f;
    public AudioSource sfxSource;

    // nhận direction từ PlayerSkill
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        transform.forward = direction;

        rb.linearVelocity = direction * speed; 
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Tự lấy AudioSource trên Main Camera nếu chưa gán
        if (sfxSource == null && Camera.main != null)
        {
            sfxSource = Camera.main.GetComponent<AudioSource>();
        }

        startPos = transform.position;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (!exploded && Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Explode();
        }
    }
    public void SetAttacker(GameObject atk)
    {
        attacker = atk;
    }

    void OnTriggerEnter(Collider other)
    {
            if (exploded) return;
            if (other.CompareTag("Player")) return;

            int layerMask = 1 << other.gameObject.layer;

            // HIT ENEMY
            if ((layerMask & enemyLayer) != 0)
            {
                Enemy1 enemy1 = other.GetComponent<Enemy1>();
                if (enemy1 != null)
                {
                    float finalDamage = damage * 100f / (100f + enemy1.def);
                    enemy1.TakeDamage((int)finalDamage);
                }

                Enemy2 enemy2 = other.GetComponent<Enemy2>();
                if (enemy2 != null)
                {
                    float finalDamage = damage * 100f / (100f + enemy2.def);
                    enemy2.TakeDamage((int)finalDamage);
                }

                Enemy3_Controller enemy3 = other.GetComponent<Enemy3_Controller>();
                if (enemy3 != null)
                {
                    float finalDamage = damage * 100f / (100f + enemy3.def);
                    enemy3.TakeDamage((int)finalDamage);
                }

                Enemy4_Controller enemy4 = other.GetComponent<Enemy4_Controller>();
                if (enemy4 != null)
                {
                    float finalDamage = damage * 100f / (100f + enemy4.def);
                    enemy4.TakeDamage((int)finalDamage);
                }

                
            }

            Boss01Health boss01Health = other.GetComponent<Boss01Health>();
            if (boss01Health != null)
            {
                Debug.Log("timf thaasy script b1");
                float finalDamage = damage;
                boss01Health.TakeDamage(finalDamage, attacker);
                Debug.Log("Đã trừ máu");
            }

            Boss02Health boss02Health = other.GetComponent<Boss02Health>();
            if (boss02Health != null)
            {
                float finalDamage = damage;
                boss02Health.TakeDamage(finalDamage, attacker);
            }
            
            if ((layerMask & groundLayer) != 0)
            {
                Explode();
            }
        }

    void Explode()
    {
        if (exploded) return;
        exploded = true;
        
        if (explosionSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(explosionSFX, explosionVolume);
        }

        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
