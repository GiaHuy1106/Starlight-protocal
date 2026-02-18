using UnityEngine;

public class BossWeaponHitbox : MonoBehaviour
{
    public int damageAmount = 20;
    public bool hasHit;
    private void OnEnable()
    {
        hasHit = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(hasHit) return;
        if(other.CompareTag("Player"))
        {
            // Giả sử Player có một script tên là PlayerHealth để quản lý máu
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if(hp != null)
            {
                hp.TakeDamage(damageAmount);
                hasHit = true;
            }
        }
    }
}
