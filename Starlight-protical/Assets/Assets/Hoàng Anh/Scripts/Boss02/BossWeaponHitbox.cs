using UnityEngine;

public class BossWeaponHitbox : MonoBehaviour
{
    public int damageAmount = 20;   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damageAmount);
                Debug.Log("Boss đánh trúng Player");
            }
        }
    }
}
