using UnityEngine;

public class PoisonArea : MonoBehaviour
{
     [Header("Poison Setting")]
    [Range(1, 100)]
    public float damagePercent = 5f;
    public float tickRate = 1f;

    [Header("Debuff VFX")]
    public GameObject poisonDebuffPrefab;

    private bool firstHit = true;
    private float nextDamageTime = 0f;

    private GameObject currentDebuff;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        firstHit = true;

        // spawn debuff effect trên đầu player
        if (poisonDebuffPrefab != null && currentDebuff == null)
        {
            Transform head = other.transform;
            currentDebuff = Instantiate(poisonDebuffPrefab, head);
            currentDebuff.transform.localPosition = new Vector3(0, 2f, 0);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time >= nextDamageTime)
        {
            PlayerHealth playerHP = other.GetComponent<PlayerHealth>();

            if (playerHP != null)
            {
                float damageAmount = playerHP.playerStats.maxHP * (damagePercent / 100f);

                playerHP.TakeDamage(damageAmount, firstHit);

                firstHit = false;

                nextDamageTime = Time.time + tickRate;
            }
        }
    }
    

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        firstHit = true;

        // xóa debuff effect
        if (currentDebuff != null)
        {
            Destroy(currentDebuff);
            currentDebuff = null;
        }
    }

    void OnDisable()
    {
        // khi poison area bị tắt hoặc enemy chết
        if (currentDebuff != null)
        {
            Destroy(currentDebuff);
            currentDebuff = null;
        }
        firstHit = true;
    }
}