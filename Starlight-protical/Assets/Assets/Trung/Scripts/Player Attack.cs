using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public PlayerInput playerInput;
    public Animator playerAnimator;
    public PlayerSkill playerSkill;

    [Header("Basic Attack")]
    public GameObject[] orbPrefabs;
    public Transform firePoint;
    private int attackHash;
    private int fireballHash;
    private int specialHash;
    private bool isAttacking;
    EnemyTargetDetector detector;

    void Start()
    {
        attackHash = Constant.AttackHash; 
        fireballHash = Constant.FireballHash;
        specialHash = Constant.SkillHash;
        detector = GetComponentInParent<EnemyTargetDetector>();
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        return;

        if (playerInput.IsInputLocked) return;
        if (playerSkill.IsAiming) return;
        if (Time.timeScale == 0f) return;

        // ⭐ chống kẹt attack
        if (isAttacking && !playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            isAttacking = false;

        if (isAttacking) return;

        HandleBasicAttackInput();
        HandleFireballAttackInput();
    }
    void HandleBasicAttackInput()
    {
        if (playerInput.IsAttackBasics())
        {
            StartBasicAttack();
        }
    }
    void HandleFireballAttackInput()
    {
        if (playerInput.IsAltHolding()) return;

        if (playerInput.IsAttackFireball() && playerSkill.IsFireBallReady)
        {
            StartFireballAttack();
        }
    }

    void StartBasicAttack()
    {
         Vector3 dir;
        if (detector != null && detector.currentTarget != null)
        {
            // Ưu tiên target enemy
            dir = detector.currentTarget.position - firePoint.position;
        }
        else
        {
            // Nếu không có target → lấy hướng chuột
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, firePoint.position);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                dir = point - firePoint.position;
            }
            else
            {
                dir = transform.forward;
            }
        }

        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        isAttacking = true;
        playerInput.SetAttackLock(true);
        playerAnimator.SetTrigger(attackHash);
    }

    public void EndBasicAttack()
    {
        isAttacking = false;
        playerInput.SetAttackLock(false);  
    }
     void StartFireballAttack()
    {
        isAttacking = true;

        playerInput.SetAttackLock(true);   

        playerAnimator.SetTrigger(fireballHash);
    }

    public void EndFireballAttack()
    {
        isAttacking = false;
        playerInput.SetAttackLock(false);  
    }

    public void StartSpecialAttack()
    {
        isAttacking = true;

        playerInput.SetAttackLock(true);   

        playerAnimator.SetTrigger(specialHash);
    }

    public void EndSpecialAttack()
    {
        isAttacking = false;
        playerInput.SetAttackLock(false);  
    }

    public void ForceStopAttack()
    {
        isAttacking = false;
        playerInput.SetAttackLock(false);  // ✅ sửa
         playerAnimator.ResetTrigger(Constant.AttackHash);
        playerAnimator.ResetTrigger(Constant.FireballHash);
        playerAnimator.ResetTrigger(Constant.SkillHash);

    }
    public void SpawnOrb()
    {
        int index = Random.Range(0, orbPrefabs.Length);

        Vector3 spawnPos = firePoint.position + transform.forward * 0.5f;

        GameObject orb = Instantiate(orbPrefabs[index], spawnPos, Quaternion.identity);
        OrbProjectile projectile = orb.GetComponent<OrbProjectile>();

        projectile.damage = playerSkill.playerStats.GetBasicDamage();

        if (detector != null && detector.currentTarget != null)
        {
            Vector3 dir = (detector.currentTarget.position - firePoint.position).normalized;
            projectile.SetDirection(dir);
            projectile.SetTarget(detector.currentTarget);
        }
        else
        {
            // bắn theo hướng chuột
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, firePoint.position);

            Vector3 dir = transform.forward;

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                dir = (point - firePoint.position).normalized;
            }

            projectile.SetDirection(dir);
        }
    }   

}
