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

        if (playerInput == null)
            playerInput = GetComponentInChildren<PlayerInput>();

        if (playerInput == null)
            playerInput = GetComponentInParent<PlayerInput>();
        }

    void Start()
    {
        playerStats.OnDamaged += PlayHurt;
        playerStats.OnDead += Die;
    }
    // hàm nhận sát thương từ các nguồn khác (enemy, trap, v.v.)
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (Time.time - lastHurtTime < hurtCooldown)
            return;

        lastHurtTime = Time.time;

        // ⭐ thêm đoạn này
        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.ForceStopAttack();

        playerInput.SetAttackLock(false);

        float finalDamage = damage - playerStats.defense;
        if (finalDamage < 1) finalDamage = 1;

        playerStats.TakeDamage((int)finalDamage);
    }

    void PlayHurt(int dmg)
    { if (isDead) return;

        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.ForceStopAttack();

        playerInput.SetAttackLock(false);
        playerInput.SetAimLock(false);

        // ⭐ QUAN TRỌNG: reset hurt lock trước
        playerInput.SetHurtLock(false);

        playerAnimator.ResetTrigger(Constant.AttackHash);
        playerAnimator.ResetTrigger(Constant.FireballHash);
        playerAnimator.ResetTrigger(Constant.SkillHash);
        playerAnimator.ResetTrigger(Constant.HurtHash);

        playerAnimator.SetTrigger(Constant.HurtHash);

        if (hurtCoroutine != null)
            StopCoroutine(hurtCoroutine);

        hurtCoroutine = StartCoroutine(HurtLockCoroutine());
    }

    IEnumerator HurtLockCoroutine()
    {
         playerInput.SetHurtLock(true);

            yield return new WaitForSeconds(hurtLockDuration);

            playerInput.SetHurtLock(false);

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
