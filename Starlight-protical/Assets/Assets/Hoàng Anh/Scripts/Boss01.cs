using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum Boss01State
{
    Patrolling,         // Đi tuần tra trong bán kính khu vực
    FollowingPlayer,    // Đuổi theo người chơi khi phát hiện
    Attacking,          // Tấn công người chơi
    Returning           // Trở về vị trí ban đầu sau khi mất dấu người chơi
}
public class Boss01 : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 300f;
    private float currentHP;
    [Header("References")]
    public Transform playerTargetTransform;
    public NavMeshAgent bossNavMeshAgent;
    public Animator bossAnimator;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;   // Prefab của đạn bắn ra
    public Transform firePoint;          // Điểm bắn đạn ra

    [Header("Skill 2 Settings")]
    //public ObjectPool icePool;   // Pool để spawn vùng cảnh báo
    public GameObject warningZone;
    public float skill2Cooldown = 20f;   // Thời gian hồi chiêu kỹ năng 2
    public float skill2Timer;              // Bộ đếm thời gian hồi chiêu kỹ năng 2

    [Header("Passive Shield Settings")]
    public GameObject shieldPrefab;
    public float shieldDuration = 5f;    // Thời gian tồn tại của lá chắn
    public float shieldCooldown = 25f;   // Thời gian hồi chiêu lá chắn
    public float slowPercent = 0.5f;       // Phần trăm giảm tốc của lá chắn

    private GameObject currentShield;        // Tham chiếu đến lá chắn đang hoạt động (nếu có)
    private float shieldCycleTimer;              // Bộ đếm thời gian hồi chiêu lá chắn
    private bool isShieldActive;           // Trạng thái lá chắn có đang hoạt động hay không

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;    // Bán kính đi tuần quanh vị trí ban đầu
    public float waitTimeAtPoint = 2f;  // Thời gian đứng chờ trước khi đi tiếp

    [Header("Follow / Return  Settings")]
    public float detectRange = 12f;    // Khoảng cách để boss phát hiện player (có thể lớn hơn chaseRange để tránh tình trạng boss mất dấu player quá nhanh)  
    public float returnRange = 15f;     // Đi quá xa vị trí ban đầu thì quay về
    public float attackDuration = 1.2f;    // Thời gian thực hiện 1 đòn tấn công

    [Header("Attack Settings")]
    public float attackRange = 2f;     // Khoảng cách tấn công
    public float attackCooldown = 2f;  // Thời gian giữa các lần tấn công

    
    private bool hasDetectedPlayer;     // Đã phát hiện player chưa

    private float attackTimer;         // Bộ đếm thời gian giữa các lần tấn công
    private bool isAttacking;        // Trạng thái tấn công
    private float attackStateTimer;    // Bộ đếm thời gian tấn công

    private Boss01State currentState = Boss01State.Patrolling;  // Trạng thái hiện tại
    private Vector3 spawnpoint;                             // Vị trí boss sinh ra ban đầu
    private Vector3 patrolTarget;                           // Điểm đi tuần hiện tại
    private float waitTimer;                               // Bộ đếm thời gian chờ

    void Start()
    {
        //bossNavMeshAgent.SetDestination(playerTargetTransform.position);
        //bossSpeedHash = Constants.speedBoss;
        currentHP = maxHP;
        // Lưu vị trí ban đầu để làm mốc đi tuần & quay về
        spawnpoint = transform.position;
        // Chọn điểm đi tuần đầu tiên
        SetNewPatrolPoint();

        bossNavMeshAgent.stoppingDistance = attackRange;
        bossNavMeshAgent.angularSpeed = 360f;
        bossNavMeshAgent.autoBraking = true;
        shieldCycleTimer = shieldCooldown; // Cho phép kích hoạt lá chắn ngay khi bắt đầu
    }
    void Update()
    {
        if (playerTargetTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTargetTransform.position);


        if (distanceToPlayer <= detectRange)
        {
            if (!hasDetectedPlayer)
            {

                hasDetectedPlayer = true;
                //Reset coooldown khi vua phat hien player
                skill2Timer = 0f;
                shieldCycleTimer = 0f;
                
                currentState = Boss01State.FollowingPlayer;

                Debug.Log("Boss Detected Player!");
            }
        }
        else
        {
            if (hasDetectedPlayer)
            {
                hasDetectedPlayer = false;
                skill2Timer = 0f;
                shieldCycleTimer = 0f;
                currentState = Boss01State.Returning;
                Debug.Log("Boss Lost Player!");
            }

        }
       
        float distanceToHome = Vector3.Distance(transform.position, spawnpoint);

        float speed = bossNavMeshAgent.velocity.magnitude;
        bossAnimator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        attackTimer += Time.deltaTime;
        if(hasDetectedPlayer)
        {
            skill2Timer += Time.deltaTime;
            shieldCycleTimer += Time.deltaTime;
        }

        // Kiểm tra điều kiện kích hoạt lá chắn    
        if (hasDetectedPlayer && !isShieldActive && shieldCycleTimer >= shieldCooldown)
        {
            ActivateShield();
            shieldCycleTimer = 0f; // Reset bộ đếm để bắt đầu chu kỳ mới
        }   
        
        switch (currentState)
        {
            case Boss01State.Patrolling:
                HandlePatrol(distanceToPlayer);
                break;

            case Boss01State.FollowingPlayer:
                HandleFollow(distanceToPlayer, distanceToHome);
                break;

            case Boss01State.Attacking:
                HandleAttack(distanceToPlayer);
                break;

            case Boss01State.Returning:
                HandleReturning();
                break;
        }
    }

    // PATROLING
    void HandlePatrol(float distanceToPlayer)
    {
        //Nếu phát hiện player trong phạm vi → chuyển trạng thái theo đuổi
        //if (distanceToPlayer <= chaseRange)
        //{
            //currentState = Boss01State.FollowingPlayer;
           // return;
       // }
        if (!bossNavMeshAgent.pathPending && bossNavMeshAgent.remainingDistance <= bossNavMeshAgent.stoppingDistance)
        {
            // Đứng chờ 1 chút cho tự nhiên
            waitTimer += Time.deltaTime;
            // Hết thời gian chờ → chọn điểm mới
            if (waitTimer >= waitTimeAtPoint)
            {
                SetNewPatrolPoint();
                waitTimer = 0f;
            }
        }
    }
    // Chọn 1 điểm ngẫu nhiên đi tuần trong bán kính
    void SetNewPatrolPoint()
    {
        // Lấy hướng ngẫu nhiên trong phạm vi patrolRadius
        Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
        randomDir += spawnpoint;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, patrolRadius, NavMesh.AllAreas))
        {
            // Lưu điểm đi tuần mới
            patrolTarget = hit.position;
            // Ra lệnh cho boss đi tới điểm đó
            bossNavMeshAgent.SetDestination(patrolTarget);
        }
    }
    // FOLLOWING PLAYER
    void HandleFollow(float distanceToPlayer, float distanceToHome)
    {
        // Nếu player trong tầm đánh
        bossNavMeshAgent.isStopped = false;

        //
        if (hasDetectedPlayer && skill2Timer >= skill2Cooldown)
        {
            UseSkill2();
            skill2Timer = 0f;
        }

        if (!isAttacking && distanceToPlayer <= attackRange + 0.1f && attackTimer >= attackCooldown)
        {
            StartAttack();
            //currentState = BossState.Attacking;
            return;
        }
        // Nếu boss bị dụ đi quá xa → quay về
        if (distanceToHome > returnRange)
        {
            currentState = Boss01State.Returning;
            return;
        }
        // Nếu player chạy mất → quay lại đi tuần
        //if (distanceToPlayer > chaseRange + 2f)
        //{
           // currentState = Boss01State.Patrolling;
           // SetNewPatrolPoint();
           // return;
       // }
        // Di chuyển liên tục về phía player
        if (!bossNavMeshAgent.hasPath || Vector3.Distance(bossNavMeshAgent.destination, playerTargetTransform.position) > 0.5f)
        {
            bossNavMeshAgent.SetDestination(playerTargetTransform.position);
        }
    }

    // Hàm bắt đầu trạng thái tấn công của Boss
    void StartAttack()
    {
        isAttacking = true;
        attackStateTimer = 0f;
        attackTimer = 0f;

        bossNavMeshAgent.ResetPath();
        bossNavMeshAgent.isStopped = true;
        bossNavMeshAgent.velocity = Vector3.zero;
        bossNavMeshAgent.updateRotation = false;
        //bossNavMeshAgent.updatePosition = false;

        Vector3 lookDir = playerTargetTransform.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        bossAnimator.SetBool("isAttacking", true);

        currentState = Boss01State.Attacking;
    }
    // ATTACKING
    void HandleAttack(float distanceToPlayer)
    {
        attackStateTimer += Time.deltaTime;
        if (distanceToPlayer > attackRange + 1.5f)
        {
            EndAttack();
            return;
        }

        Vector3 lookDir = playerTargetTransform.position - transform.position;
        lookDir.y = 0;

        //Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        //transform.rotation = Quaternion.LookRotation(lookDir);
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }
        if (attackStateTimer >= attackDuration)
        {
            EndAttack();
        }
    }
    public void EndAttack()
    {
        isAttacking = false;

        bossAnimator.SetBool("isAttacking", false);

        bossNavMeshAgent.updateRotation = true;
        bossNavMeshAgent.isStopped = false;

        currentState = Boss01State.FollowingPlayer;
    }

    //Spawn đạn từ animation event
    public void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        BossProjectile projectile = proj.GetComponent<BossProjectile>();
        projectile.Initialize(playerTargetTransform);

    }
    void UseSkill2()
    {
        if (warningZone == null || playerTargetTransform == null) return;

        //
        bossNavMeshAgent.isStopped = true;
        //xoay mặt về hướng player
        Vector3 lookDir = playerTargetTransform.position - transform.position;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
        //Spawn vùng cảnh báo quanh player
        for (int i = 0; i < 8; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-60f, 60f),
                0,
                Random.Range(-60f, 60f)
                );
            Vector3 targetPos = playerTargetTransform.position + randomOffset;
            Instantiate(warningZone, targetPos, Quaternion.identity);
            //GameObject ice = icePool.GetObject();
            //ice.transform.position = targetPos;
            //ice.transform.rotation = Quaternion.identity;
        }
        bossNavMeshAgent.isStopped = false;
    }
    void ActivateShield()
    {
        isShieldActive = true;

        currentShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
        currentShield.transform.SetParent(transform);
        currentShield.transform.localPosition = Vector3.zero;
        //currentShield.transform.localRotation = Quaternion.identity;
        //currentShield.transform.localScale = Vector3.one;

        shieldCycleTimer = 0f;

        StartCoroutine(ShieldDurationCoroutine());

        Debug.Log("Passive Shield Activated!");
    }
    IEnumerator ShieldDurationCoroutine()
    {
        yield return new WaitForSeconds(shieldDuration);

        DeactivateShield();
    }
    void DeactivateShield()
    {
        isShieldActive = false;

        if (currentShield != null)
            Destroy(currentShield);

        shieldCycleTimer = 0f; // BẮT ĐẦU 30s cooldown từ đây

        Debug.Log("Shield Ended - Cooldown Started");
    }
    // RETURNING
    void HandleReturning()
     {
        // Quay về vị trí ban đầu
        bossNavMeshAgent.SetDestination(spawnpoint);
        // Nếu đã về tới nơi → tiếp tục đi tuần
        if (!bossNavMeshAgent.pathPending && bossNavMeshAgent.remainingDistance <= bossNavMeshAgent.stoppingDistance)
        {
            currentState = Boss01State.Patrolling;
            SetNewPatrolPoint();
        }
     }
    public void TakeDamege(float damage, GameObject attacker)
    {
        currentHP -= damage;
        Debug.Log("Boss HP: " + currentHP + " / " + maxHP);
        //Nếu shield đang bật => làm chậm player
        if (isShieldActive && attacker != null)
        {
            PlayerMovement player = attacker.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.ApplySlow(slowPercent, 2f);
            }
            
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Boss Đie");
        bossNavMeshAgent.isStopped = true;

        if(currentShield != null) Destroy(currentShield);

    }    
      // ====== DEBUG ======
    void OnDrawGizmosSelected()
    {
        //Bán kính đi tuần
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        //Khoảng cách phát hiện người chơi
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        //Khoảng cách trở về vị trí ban đầu
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, returnRange);
    }
}

