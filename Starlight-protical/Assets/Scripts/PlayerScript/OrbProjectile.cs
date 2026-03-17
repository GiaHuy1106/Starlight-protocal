using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OrbProjectile : MonoBehaviour
{   
    public GameObject explosionPrefab;
    public float speed = 10f;
    public int damage; // damage nhận từ player
    public float maxDistance = 10f;
    public LayerMask enemyLayer;
    Rigidbody rb;
    Vector3 startPos;
    Vector3 direction;
    Transform target;
    bool exploded = false;
    GameObject attacker;
    public AudioClip explosionSFX; // âm thanh nổ
    [Range(0f, 1f)]
    public float explosionVolume = 1f;
    public AudioSource sfxSource;  // AudioSource dùng chung để PlayOneShot
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        // Tự tìm AudioSource trên Main Camera nếu chưa gán trong Inspector
        if (sfxSource == null && Camera.main != null)
        {
            sfxSource = Camera.main.GetComponent<AudioSource>();
        }

        startPos = transform.position;
        Collider playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
        Collider orbCol = GetComponent<Collider>();

        Physics.IgnoreCollision(playerCol, orbCol);
        Destroy(gameObject, 5f);
    }   
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.forward = direction;
        rb.linearVelocity = direction * speed; 
    }
        public void SetTarget(Transform enemy)
        {
            target = enemy;
        }
    void Update()
    {if (exploded) return;

        // nếu có target → tracking
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;

            rb.linearVelocity = dir * speed;

            transform.forward = dir;
        }
        // nếu bay quá xa → nổ
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
         Debug.Log("Hit: " + other.name);
        if (exploded) return;

        
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Enemy1 enemy = other.GetComponent<Enemy1>();
            if (enemy != null)
            {
                float finalDamage = damage * 100f / (100f + enemy.def); // công thức tính damage có phòng thủ của enemy
                enemy.TakeDamage((int)finalDamage);
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
            Boss01Health boss01Health = other.GetComponent<Boss01Health>();
            if (boss01Health != null)
            {
                float finalDamage = damage;
                boss01Health.TakeDamage(finalDamage, attacker);
            }
           Boss02Health boss02Health = other.GetComponent<Boss02Health>();
            if (boss02Health != null)
            {
                float finalDamage = damage;
                boss02Health.TakeDamage(finalDamage, attacker);
            }
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

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
