using UnityEngine;
using UnityEngine.UI;

public class FakePlayerHealth : MonoBehaviour
{
    [Header("Cài đặt Máu")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("UI")]
    public Slider healthBar;

    void Start()
    {
        // 1. Khởi tạo máu đầy
        currentHP = maxHP;

        // 2. Setup thanh máu ban đầu (nếu có)
        UpdateHealthBar();
    }

    // Hàm này sẽ được Enemy gọi
    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        Debug.Log($"Player bị đánh! Mất {damage} máu. Còn lại: {currentHP}");

        // Cập nhật thanh máu
        UpdateHealthBar();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            // Slider chạy từ 0 đến 1
            healthBar.value = currentHP / maxHP;
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER! Player đã chết.");
        gameObject.SetActive(false); 
    }
}