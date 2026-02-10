using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkill : MonoBehaviour
{
    public PlayerStats playerStats;

    [Header("Input")]
    public PlayerInput playerInput;

    [Header("Special Spawn")]
    public float specialDistance = 5f;

    [Header("Prefabs")]
    public GameObject fireballPrefab;
    public GameObject specialPrefab;

    [Header("Spawn")]
    public Transform firePoint;

    // ⭐ Glow tip
    [Header("Glow")]
    public WandGlow wandGlow;

    [Header("Basic Cooldown")]
    public float basicCooldown = 1f;
    float basicCooldownTimer;
    public bool IsBasicReady => basicCooldownTimer <= 0f;

    [Header("Special Cooldown")]
    public float specialCooldown = 5f;
    float cooldownTimer;
    public bool IsSpecialReady => cooldownTimer <= 0f;

    [Header("UI")]
    public Image specialcooldownMask;
    public Image basicCooldownMask;

    [Header("Mana Cost")]
    public int specialManaCost = 40;

    [Header("Skill Icon")]
    public Image specialIcon;
    public float disableAlpha = 0.3f; // 
    void Update()
    {
        UpdateBasicCooldown();
        UpdateSpecialCooldownUI();
        UpdateBasicCooldownUI();
        UpdateSpecialIconState();
    }
    public void CastSpecial()
    {
        // cooldown
        if (!IsSpecialReady) return;

        // thiếu mana
        if (playerStats.CurrentMana < specialManaCost) return;
        // trừ mana
        playerStats.UseMana(specialManaCost);

        Vector3 spawnPos = transform.position + transform.forward * specialDistance;

        GameObject go = Instantiate(specialPrefab, spawnPos, Quaternion.identity);

        MeterorSkillDamage meteor = go.GetComponent<MeterorSkillDamage>();
        meteor.damage = playerStats.GetSpecialDamage();

        cooldownTimer = specialCooldown;
    }

    // Cập nhật trạng thái icon kỹ năng đặc biệt
    void UpdateSpecialIconState()
    {
        bool notEnoughMana = playerStats.CurrentMana < specialManaCost;
        bool onCooldown = cooldownTimer > 0f;

        float alpha = (notEnoughMana || onCooldown) ? disableAlpha : 1f;
        
        Color color = specialIcon.color;
        color.a = alpha;
        specialIcon.color = color;
    }
    public void GlowOn()
    {
        if (wandGlow != null)
            wandGlow.SetGlow(true);
    }

    public void GlowOff()
    {
        if (wandGlow != null)
            wandGlow.SetGlow(false);
    }

    public void ShootFireball()
    {
        if (!IsBasicReady) return;

        Vector3 dir = firePoint.forward;
         // ⭐ đẩy ra trước để không va Player
        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;

        GameObject go = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        FireballProjectile projectile = go.GetComponent<FireballProjectile>();
        projectile.damage = playerStats.GetBasicDamage();
        projectile.SetDirection(dir);

        basicCooldownTimer = basicCooldown;
    }

    void UpdateBasicCooldown()
    {
        if (basicCooldownTimer > 0f)
            basicCooldownTimer -= Time.deltaTime;
    }
    void UpdateBasicCooldownUI()
{
    if (basicCooldownTimer > 0f)
    {
        basicCooldownMask.fillAmount = basicCooldownTimer / basicCooldown;
    }
    else
    {
        basicCooldownMask.fillAmount = 0f;
    }
}

    void UpdateSpecialCooldownUI()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            specialcooldownMask.fillAmount = cooldownTimer / specialCooldown;
        }
        else
        {
            specialcooldownMask.fillAmount = 0f;
        }
    }
}
