using UnityEngine;

public class BossKickHitbox : MonoBehaviour
{
    public int damageAmount = 30;
    bool hasHit = false; // Tránh va chạm nhiều lần
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
                       
        if (other.CompareTag("Player"))
        {
            
            PlayerHealth hp = other.GetComponentInParent<PlayerHealth>();
            if (hp != null)
            {
                hasHit = true;
                hp.TakeDamage(damageAmount);
                Debug.Log("Boss Kick đánh trúng Player");
            }
        }
    }
}
