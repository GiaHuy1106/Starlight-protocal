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

    [Header("Fireball Cooldown")]
    public float fireballCooldown = 5f;
    float fireballCooldownTimer;
    public bool IsFireBallReady => fireballCooldownTimer <= 0f;

    [Header("Special Cooldown")]
    public float specialCooldown = 10f;
    float SpecialCooldownTimer;
    public bool IsSpecialReady => SpecialCooldownTimer <= 0f;

    [Header("UI")]
    public Image specialcooldownMask;
    public Image fireballCooldownMask;

    [Header("Mana Cost")]
    public int fireballManaCost = 10;
    public int specialManaCost = 40;

    [Header("Skill Icon")]
    public Image specialIcon;
    public Image fireballIcon;
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
        UpdateFireballCooldown();
        UpdateSpecialCooldownUI();
        UpdateFireballCooldownUI();
        UpdateFireballIConState();
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

        playerAttack.StartSpecialAttack();
    }
        public void SpawnSpecial()
    {
        if (targetIndicator == null) return;

        Vector3 spawnPos = targetIndicator.transform.position;

        Instantiate(specialPrefab, spawnPos, Quaternion.identity);

        SpecialCooldownTimer = specialCooldown;
        

        StopAim();
    }

    void UpdateFireballIConState()
    {
        bool notEnoughMana = playerStats.CurrentMana < fireballManaCost;
        bool onCooldown = fireballCooldownTimer > 0f;
        float alpha = (notEnoughMana || onCooldown) ? disableAlpha : 1f;
        Color color = fireballIcon.color;
        color.a = alpha;
        fireballIcon.color = color;
    }

    // Cập nhật trạng thái icon kỹ năng đặc biệt
    void UpdateSpecialIconState()
    {
        bool notEnoughMana = playerStats.CurrentMana < specialManaCost;
        bool onCooldown = SpecialCooldownTimer > 0f;

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
        if (!IsFireBallReady) return;
        if (playerStats.CurrentMana < fireballManaCost)
        {
            UIMessage.Instance.Show("Not enough mana");
            return;
        }

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
        // Trừ mana trước khi bắn
        playerStats.UseMana(fireballManaCost);

        // Spawn đạn
        GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        FireballProjectile projectile = go.GetComponent<FireballProjectile>();
        projectile.damage = playerStats.GetBasicDamage();
        projectile.SetDirection(dir);

        fireballCooldownTimer = fireballCooldown;
    }


    void UpdateFireballCooldown()
    {
        if (fireballCooldownTimer > 0f)
            fireballCooldownTimer -= Time.deltaTime;
    }
    void UpdateFireballCooldownUI()
    {
        if (fireballCooldownTimer > 0f)
        {
            fireballCooldownMask.fillAmount = fireballCooldownTimer / fireballCooldown;
        }
        else
        {
            fireballCooldownMask.fillAmount = 0f;
        }
    }

    void UpdateSpecialCooldownUI()
    {
        if (SpecialCooldownTimer > 0f)
        {
            SpecialCooldownTimer -= Time.deltaTime;
            specialcooldownMask.fillAmount = SpecialCooldownTimer / specialCooldown;
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
        
        rangeCircle = Instantiate(rangeCirclePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        rangeCircle.transform.localScale = Vector3.one * meteorRange * rangeCircleScaleFactor;

        targetIndicator = Instantiate(targetIndicatorPrefab);
    }

    // bật indicator ở đây nếu có
    public void StopAim()
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
