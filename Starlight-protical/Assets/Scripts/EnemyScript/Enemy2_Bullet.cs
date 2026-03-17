using UnityEngine;

public class Enemy2_Bullet : MonoBehaviour
{
    public float speed = 10f;      // Tốc độ bay
    public float damage;     // Sát thương 
    public float lifeTime = 3f;    // Thời gian tồn tại (để không bay mãi mãi)

    void Start()
    {
        // Tự hủy sau 5 giây nếu không trúng gì để đỡ lag game
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Bay thẳng về phía trước theo hướng của nó
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Nếu va chạm với Player
        if (other.CompareTag("Player"))
        {
            // Gọi hàm trừ máu của Player
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Hủy viên đạn sau khi trúng
            Destroy(gameObject);
        }
    }
}
