using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkill : MonoBehaviour
{
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
    public Image cooldownMask;
    void Update()
    {
        UpdateBasicCooldown();
        UpdateCooldownUI();
    }
    public void CastSpecial()
    {
        if (!IsSpecialReady) return;

        Vector3 spawnPos = transform.position + transform.forward * specialDistance;

        Instantiate(specialPrefab, spawnPos, Quaternion.identity);

        cooldownTimer = specialCooldown;
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

        GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        FireballProjectile projectile = go.GetComponent<FireballProjectile>();
        projectile.SetDirection(dir);

        basicCooldownTimer = basicCooldown;
    }

    void UpdateBasicCooldown()
    {
        if (basicCooldownTimer > 0f)
            basicCooldownTimer -= Time.deltaTime;
    }

    void UpdateCooldownUI()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownMask.fillAmount = cooldownTimer / specialCooldown;
        }
        else
        {
            cooldownMask.fillAmount = 0f;
        }
    }
}
