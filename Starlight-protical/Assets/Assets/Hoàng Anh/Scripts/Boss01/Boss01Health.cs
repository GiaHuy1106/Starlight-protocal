using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss01Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 100f;
    public Slider healthSlider;
    public Slider easeHealthSlider;

    [Header("RegenerateHealth")]
    public float regenSpeed = 5f; // máu hồi mỗi giây
    private Coroutine regenCoroutine;

    private float currentHP;
    private float lerpSpeed = 0.05f;

    bool isDead = false;
    private Boss01 boss;
    void Start()
    {
        currentHP = maxHP;
        healthSlider.maxValue = maxHP;
        easeHealthSlider.maxValue = maxHP;

        healthSlider.value = maxHP;
        easeHealthSlider.value = maxHP;

        healthSlider.gameObject.SetActive(false);
        easeHealthSlider.gameObject.SetActive(false);
        boss = GetComponent<Boss01>();
    }
    private void Update()
    {
        if(healthSlider.value != currentHP)
        {
            healthSlider.value = currentHP;
        }  
        if(easeHealthSlider.value != healthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHP, lerpSpeed);
        }    
        
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
    IEnumerator RegenerateHealth()
    {
        while (currentHP < maxHP)
        {
            currentHP += regenSpeed * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            yield return null;
        }
    }
    public void ShowHealthBar()
    {
        healthSlider.gameObject.SetActive(true);
        easeHealthSlider.gameObject.SetActive(true);
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
    }
    public void HideHealthBar()
    {
        healthSlider.gameObject.SetActive(false);
        easeHealthSlider.gameObject.SetActive(false);
        if (regenCoroutine == null && currentHP < maxHP)
        {
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    void Die()
    {
        if(isDead) return; //chặn animation Die nếu boss đã chết
        isDead = true;

        // Ẩn UI thanh máu khi boss chết
        HideHealthBar();

        boss.Die();
    }    
}
