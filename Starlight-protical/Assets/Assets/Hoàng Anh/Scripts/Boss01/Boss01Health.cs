using UnityEngine;

public class Boss01Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 100f;
    private float currentHP;
    bool isDead = false;
    private Boss01 boss;
    void Start()
    {
        currentHP = maxHP;
        boss = GetComponent<Boss01>();
    }
    public void TakeDamage(float damage, GameObject attacker)
    {
        if (isDead) return; //chặn animation GetHit nếu boss đã chết
        currentHP -= damage;

        Debug.Log("Boss HP: " + currentHP + " / " + maxHP);
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
                player.ApplySlow(boss.slowPercent, boss.slowDuration);
            }    
        }    
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        if(isDead) return; //chặn animation Die nếu boss đã chết
        isDead = true;
        boss.Die();
    }    
}
