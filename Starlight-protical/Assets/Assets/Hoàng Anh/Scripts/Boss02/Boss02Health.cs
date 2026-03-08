using UnityEngine;

public class Boss02Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 100f;
    private float currentHP;

    private Boss02 boss;
    bool isDead = false;
    void Start()
    {
        currentHP = maxHP;
        boss = GetComponent<Boss02>();
    }
    public void TakeDamage(float damage, GameObject attacker)
    {
        if(isDead) return;
        currentHP -= damage;

        Debug.Log("Boss02 HP: " + currentHP + " / " + maxHP);
        if(boss != null)
        {
            boss.PlayerGetHit();
            //KnockBack
            boss.KnockBack(attacker.transform.position, 0.5f);
        }    
        if (boss != null && boss.IsShieldActive())
        {
            PlayerMovement player = attacker.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.ApplySlow(0.5f, 5f);
            }
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        if(isDead) return;
        isDead = true;
        boss.Die();
    }
}
