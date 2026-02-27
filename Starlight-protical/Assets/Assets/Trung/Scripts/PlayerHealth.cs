using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{ 
     public PlayerStats playerStats;
    public Animator playerAnimator;

    [Header("Hurt Settings")]
    public float hurtCooldown = 0.5f;      // thời gian miễn nhiễm
    public float hurtLockDuration = 0.3f;  // thời gian khóa input
    private Coroutine hurtCoroutine;
    private float lastHurtTime;
    private bool isDead;
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        playerStats.OnDamaged += PlayHurt;
        playerStats.OnDead += Die;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (Time.time - lastHurtTime < hurtCooldown)
            return;

        lastHurtTime = Time.time;

        playerStats.TakeDamage((int)damage);
    }

    void PlayHurt(int dmg)
    {
        if (isDead) return;
        GetComponent<PlayerAttack>().ForceStopAttack();

        playerAnimator.ResetTrigger(Constant.HurtHash);
        playerAnimator.SetTrigger(Constant.HurtHash);

        if (hurtCoroutine != null)
            StopCoroutine(hurtCoroutine);

        hurtCoroutine = StartCoroutine(HurtLockCoroutine());
    }

    IEnumerator HurtLockCoroutine()
    {
        
        playerInput.SetHurtLock(true);   // ✅ đổi

        yield return new WaitForSeconds(hurtLockDuration);

        if (!isDead)
            playerInput.SetHurtLock(false);  // ✅ đổi

        hurtCoroutine = null;
    }

    void Die()
    {
         if (isDead) return;

        isDead = true;

        playerAnimator.ResetTrigger(Constant.HurtHash);
        playerAnimator.SetTrigger(Constant.DieHash);

        playerInput.SetHurtLock(true);  // ✅ đổi

        GetComponent<PlayerMovement>().enabled = false;
    }
}
