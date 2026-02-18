using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player bị trúng đòn! Máu còn: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Player Died");
        // TODO: animation chết / respawn / game over
        // Add death logic here
        gameObject.SetActive(false);
    }
}
