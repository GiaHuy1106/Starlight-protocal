using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{ 
     public PlayerStats playerStats;
    public Animator playerAnimator;

    [Header("Hurt Sound")]
    public AudioClip hurtVoice;
    [Range(0f,1f)]
    public float hurtVolume = 1f;
    public AudioSource voiceSource;

    [Header("Death Sound")]
    public AudioClip deathVoice;
    [Range(0f,1f)]
    public float deathVolume = 1f;

    [Header("Hurt Settings")]
    public float hurtCooldown = 0.5f;      // thời gian miễn nhiễm
    public float hurtLockDuration = 0.3f;  // thời gian khóa input
    private Coroutine hurtCoroutine;
    private float lastHurtTime;
    private bool isDead;
    bool ignoreNextHurt = false;
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
        if (voiceSource == null && Camera.main != null)
        {
            voiceSource = Camera.main.GetComponent<AudioSource>();
        }
    }
    // hàm nhận sát thương từ các nguồn khác (enemy, trap, v.v.)
    public void TakeDamage(float damage, bool playHurt = true)
    {
        if (isDead) return;

        if (Time.time - lastHurtTime < hurtCooldown)
            return;

        lastHurtTime = Time.time;
        float finalDamage = damage - playerStats.defense;
        if (finalDamage < 1) finalDamage = 1;
        
        // ⭐ nếu damage này làm chết player → không trigger hurt
        bool willDie = playerStats.CurrentHP - finalDamage <= 0;

        playerStats.TakeDamage((int)finalDamage, !willDie && playHurt);
        if (!playHurt) return;

        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.ForceStopAttack();

        playerInput.SetAttackLock(false);
    }

    void PlayHurt(int dmg)
    { 
        if (isDead) return;
        // ⭐ play hurt voice
        if (hurtVoice != null && voiceSource != null)
        {
            voiceSource.PlayOneShot(hurtVoice, hurtVolume);
        }
        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.ForceStopAttack();

        playerInput.SetAttackLock(false);
        playerInput.SetAimLock(false);

        // reset hurt lock trước
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

        // ⭐ phát âm thanh chết
        if (deathVoice != null && voiceSource != null)
        {
            voiceSource.PlayOneShot(deathVoice, deathVolume);
        }

        playerAnimator.ResetTrigger(Constant.HurtHash);
        playerAnimator.SetTrigger(Constant.DieHash);

        playerInput.SetHurtLock(true);  // ✅ đổi

        GetComponent<PlayerMovement>().enabled = false;
    }

}
