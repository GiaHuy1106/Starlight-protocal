using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 10;
    public float lifeTime = 5f;
    public GameObject hitVFX;

    private Vector3 moveDirection; //lưu hướng bay cố định
    private bool hasHit = false; // Tránh va chạm nhiều lần

    public void Initialize(Transform target)
    {
        // Lấy hướng bay
        moveDirection = (target.position - transform.position).normalized;
        //Xoay đạn theo hướng bay
        transform.rotation = Quaternion.LookRotation(moveDirection);
        // Hủy đạn sau một khoảng thời gian
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {          
        transform.position += moveDirection * speed * Time.deltaTime;       
    }

    void OnTriggerEnter(Collider other)
    {       
        if (hasHit) return; // Nếu đã va chạm, không xử lý tiếp

        hasHit = true;

        // Tắt collider để tránh trigger nhiều lần
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null)
            player = other.GetComponentInParent<PlayerHealth>(); // Kiểm tra nếu collider là con của player

        if (player != null)
        {
            //Debug.Log("Hit Player");
            player.TakeDamage(damage);         
        }
        // Spawn VFX cho mọi va chạm (player, tường, mặt đất)       
        Instantiate(hitVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
