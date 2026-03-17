using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MeterorSkillDamage : MonoBehaviour
{
    public int damage = 50;
    public float lifetime = 5f;        // Thời gian tồn tại của thiên thạch
    public float tickInterval = 0.5f;  // Thời gian giữa mỗi lần gây damage
    public LayerMask enemyLayer;       // Layer của enemy để lọc va chạm

    [Header("SFX")]
    public AudioClip fallSFX;          // tiếng whoosh khi mưa thiên thạch bắt đầu
    public AudioClip hitSFX;           // tiếng nổ khi va chạm / gây damage
    [Range(0f, 1f)]
    public float fallVolume = 1f;
    [Range(0f, 1f)]
    public float hitVolume = 1f;
    public AudioSource sfxSource;      // thường dùng AudioSource trên Main Camera
    GameObject attacker;
    void Start()
    {
        Destroy(gameObject, lifetime);
        
        // Tự lấy AudioSource trên Main Camera nếu chưa gán
        if (sfxSource == null && Camera.main != null)
        {
            sfxSource = Camera.main.GetComponent<AudioSource>();
        }

        // Phát tiếng whoosh khi bắt đầu rơi
        if (fallSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(fallSFX, fallVolume);
        }

        StartCoroutine(DamageOverTimeArea());
    }

    IEnumerator DamageOverTimeArea()
    {
        float elapsed = 0f;

        // Lấy bán kính từ SphereCollider nếu có, nếu không dùng giá trị mặc định
        float radius = 1.5f;
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            radius = sphere.radius * scale;
        }

        while (elapsed < lifetime)
        {
            DealDamageInArea(radius);

            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
    }

    void DealDamageInArea(float radius)
    {
        Collider[] hits;

        if (enemyLayer.value != 0)
        {
            hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        }
        else
        {
            hits = Physics.OverlapSphere(transform.position, radius);
        }

        foreach (var hit in hits)
        {
            Enemy1 enemy1 = hit.GetComponent<Enemy1>();
            if (enemy1 != null)
            {
                float finalDamage = damage * 100f / (100f + enemy1.def);
                enemy1.TakeDamage((int)finalDamage);
            }

            Enemy2 enemy2 = hit.GetComponent<Enemy2>();
            if (enemy2 != null)
            {
                float finalDamage = damage * 100f / (100f + enemy2.def);
                enemy2.TakeDamage((int)finalDamage);
            }

            Enemy3_Controller enemy3 = hit.GetComponent<Enemy3_Controller>();
            if (enemy3 != null)
            {
                float finalDamage = damage * 100f / (100f + enemy3.def);
                enemy3.TakeDamage((int)finalDamage);
            }

            Enemy4_Controller enemy4 = hit.GetComponent<Enemy4_Controller>();
            if (enemy4 != null)
            {
                float finalDamage = damage * 100f / (100f + enemy4.def);
                enemy4.TakeDamage((int)finalDamage);
            }
            Boss01Health boss01Health = hit.GetComponent<Boss01Health>();
            if (boss01Health != null)
            {
                float finalDamage = damage;
                boss01Health.TakeDamage(finalDamage, attacker);
            }
            Boss02Health boss02Health = hit.GetComponent<Boss02Health>();
            if (boss02Health != null)
            {
                float finalDamage = damage;
                boss02Health.TakeDamage(finalDamage, attacker);
            }
        }
        // Phát tiếng nổ cho mỗi "wave" meteors rơi, dù có trúng enemy hay không
        if (hitSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(hitSFX, hitVolume);
        }

    }
    public void SetAttacker(GameObject atk)
    {
        attacker = atk;
    }

}
