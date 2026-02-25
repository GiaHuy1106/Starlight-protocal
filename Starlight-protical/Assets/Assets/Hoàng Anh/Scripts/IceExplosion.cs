using UnityEngine;

public class IceExplosion : MonoBehaviour
{
    public float damageRadius = 5f;
    public int damage = 40;
    public LayerMask playerLayer;

    void Start()
    {
        DealDamage();
        float lifetime = GetComponent<ParticleSystem>().main.duration
                   + GetComponent<ParticleSystem>().main.startLifetime.constantMax;

        Destroy(gameObject, 2f);
    }
    void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Hits count: " + hits.Length);
                Debug.Log("Ice Explosion hit Player");
                playerHealth.TakeDamage(damage);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
