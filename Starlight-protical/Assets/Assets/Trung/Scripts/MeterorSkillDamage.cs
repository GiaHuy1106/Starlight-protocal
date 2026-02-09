using UnityEngine;

public class MeterorSkillDamage : MonoBehaviour
{
    public int damage = 50;
    public float lifetime = 5f; // Thời gian tồn tại của thiên thạch

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    
    void OnTriggerEnter(Collider other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
        }
    }
}
