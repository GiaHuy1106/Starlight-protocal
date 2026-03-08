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

        Transform vfxPoint = other.GetComponentInChildren<Transform>(true);

        foreach (Transform t in other.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == "VFX_Point")
            {
                vfxPoint = t;
                break;
            }
        }

        if (poisonDebuffPrefab != null && currentDebuff == null && vfxPoint != null)
        {
            currentDebuff = Instantiate(poisonDebuffPrefab, vfxPoint);
            currentDebuff.transform.localPosition = Vector3.zero;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time >= nextDamageTime)
        {
            PlayerHealth playerHP = other.GetComponentInParent<PlayerHealth>();

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
}