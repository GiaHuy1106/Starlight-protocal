using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    [Header("Poison Setting")]
    [Range(1, 100)]
    public float damagePercent = 5f;
    public float tickRate = 1f; // 1 giây rút 1 lần

    private float nextDamageTime = 0f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Kiểm tra xem đã đến lúc rút máu chưa
            if (Time.time >= nextDamageTime)
            {
                FakePlayerHealth playerHP = other.GetComponent<FakePlayerHealth>();

                if (playerHP != null)
                {
                    float damageAmount = playerHP.maxHP * (damagePercent / 100f);
                    playerHP.TakeDamage(damageAmount);
                    Debug.Log($"Độc rút {damageAmount} máu!");

                    // Set thời gian cho lần rút tiếp theo
                    nextDamageTime = Time.time + tickRate;
                }
            }
        }
    }
}