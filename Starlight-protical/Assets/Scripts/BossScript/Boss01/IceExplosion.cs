using UnityEngine;

public class IceExplosion : MonoBehaviour
{
    public float damageRadius = 5f;
    public int damage = 40;
    public LayerMask playerLayer;

    [Header("Audio Settings")]
    public AudioClip explosionSound;
    private AudioSource audioSource;

    void Start()
    {
        //Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if(explosionSound != null)
        {
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.minDistance = 3f;
            audioSource.maxDistance = 30f;
            audioSource.dopplerLevel = 0f;

            audioSource.pitch = Random.Range(0.9f, 1.8f); // Thêm chút ngẫu nhiên cho âm thanh
            audioSource.PlayOneShot(explosionSound);
        }
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
