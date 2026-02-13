using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkill : MonoBehaviour
{
    public TargetSystem targetSystem;
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

        Transform target = targetSystem != null ? targetSystem.GetTarget() : null;
        Vector3 dir;

        if (target != null)
        {
            // Có target → bắn vào target (chỉ tính ngang)
            dir = target.position - transform.position;
        }
        else
        {
            // Không có target → bắn theo chuột trên mặt phẳng đất (Y = 0)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // mặt đất thật

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                dir = point - transform.position;
            }
            else
            {
                dir = transform.forward;
            }
        }

        //  CHỈ GIỮ HƯỚNG NGANG
        dir.y = 0f;
        dir.Normalize();

        //  Quay player theo hướng bắn
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        //  Spawn đạn
        Vector3 spawnPos = firePoint.position;

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
