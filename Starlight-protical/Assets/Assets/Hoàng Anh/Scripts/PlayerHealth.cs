using UnityEditor.Experimental.GraphView;
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
        Debug.Log("Damage nhận: " + damage);
        currentHealth -= damage;      
        Debug.Log("Máu còn: " + currentHealth);
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
