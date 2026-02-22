using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.VersionControl;

public class PlayerSkill : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerAttack playerAttack;

    [Header("Input")]
    public PlayerInput playerInput;

    [Header("Special Spawn")]
    public float specialDistance = 5f;

    [Header("Prefabs")]
    public GameObject fireballPrefab;
    public GameObject specialPrefab;

    [Header("Spawn")]
    public Transform firePoint;

    // Glow tip
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
    [Header("Meteor Range")]
    public float meteorRange = 8f;
    public float rangeCircleScaleFactor = 0.14f;
    [Header("Indicator")]
    public GameObject rangeCirclePrefab;
    public GameObject targetIndicatorPrefab;
    GameObject rangeCircle;
    GameObject targetIndicator;
    public bool isAimingSkill; // nút nhắm kỹ năng special
    
    void Start()
    {
        isAimingSkill = false;
    }
    void Update()
    {
        UpdateBasicCooldown();
        UpdateSpecialCooldownUI();
        UpdateBasicCooldownUI();
        UpdateSpecialIconState();
        HandleSkillInput();
    }
    public void CastSpecial()
    {
         if (!isAimingSkill) return;
        if (targetIndicator == null) return;

        if (!IsSpecialReady) return;

        if (playerStats.CurrentMana < specialManaCost)
        {
            UIMessage.Instance.Show("Not enough mana");
            return;
        }

        playerStats.UseMana(specialManaCost);

        Vector3 spawnPos = targetIndicator.transform.position;

        Vector3 dir = spawnPos - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // khóa input trong lúc cast
        playerInput.SetInputLock(true);

        playerAttack.StartSpecialAttack();
    }
        public void SpawnSpecial()
    {
        if (targetIndicator == null) return;

        Vector3 spawnPos = targetIndicator.transform.position;

        Instantiate(specialPrefab, spawnPos, Quaternion.identity);

        cooldownTimer = specialCooldown;

        StopAim();
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
        if (Time.timeScale == 0f) return;
        if (!IsBasicReady) return;

        Vector3 dir;

        // Ray từ camera xuống mặt đất
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, firePoint.position);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                dir = point - firePoint.position;
            }
            else
            {
                dir = transform.forward;
            }

            dir.y = 0f;
            dir.Normalize();

        // Xoay player theo hướng chuột
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // Spawn đạn
        GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

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

    void HandleSkillInput()
    {
        // Bắt đầu aim
        if (!isAimingSkill)
        {
            if (playerInput.IsAimSkill() && IsSpecialReady)
            {
                StartAim();
            }
            return;
        }
        // đang aim
        if (playerInput.IsConfirmSkill())
        {
            CastSpecial();
        }
        if (playerInput.IsCancelSkill())
        {
            StopAim();
        }
    }
    void StartAim()
    {
        // kiểm tra mana trước khi cho aim
        if (playerStats.CurrentMana < specialManaCost)
        {
            UIMessage.Instance.Show("Not enough mana");
            return;
        }
        isAimingSkill = true;
        playerInput.SetInputLock(true);
        rangeCircle = Instantiate(rangeCirclePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        rangeCircle.transform.localScale = Vector3.one * meteorRange * rangeCircleScaleFactor;

        targetIndicator = Instantiate(targetIndicatorPrefab);
    }

    // bật indicator ở đây nếu có
    void StopAim()
    {
        isAimingSkill = false;
        if (rangeCircle) Destroy(rangeCircle);
        if (targetIndicator) Destroy(targetIndicator);
    }
    public bool IsAiming => isAimingSkill;
    void LateUpdate()
    {
        if (!isAimingSkill) return;

        UpdateIndicatorPosition();

        if (rangeCircle != null)
        rangeCircle.transform.position = transform.position;
    }
    void UpdateIndicatorPosition()
    {
        if (targetIndicator == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            Vector3 dir = point - transform.position;
            dir.y = 0;

            // Giới hạn trong bán kính
            dir = Vector3.ClampMagnitude(dir, meteorRange);

            Vector3 finalPos = transform.position + dir;

            targetIndicator.transform.position = finalPos;
        }
    }
}
