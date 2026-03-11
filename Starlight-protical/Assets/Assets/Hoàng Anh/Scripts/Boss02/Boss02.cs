using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public enum Boss02State
{
    Patrolling,         // Đi tuần tra trong bán kính khu vực
    FollowingPlayer,    // Đuổi theo người chơi khi phát hiện
    Attacking,          // Tấn công người chơi
    Returning           // Trở về vị trí ban đầu sau khi mất dấu người chơi
}
public class Boss02 : MonoBehaviour, IShieldable
{
    [Header("References")]
    public Transform playerTargetTransform;
    public NavMeshAgent bossNavMeshAgent;
    public Animator bossAnimator;
    public Collider weaponHitBox;
    public Collider kickHitBox;

    [Header("Audio")]
    public AudioClip jumpLandSound;
    public AudioClip punchSound;
    public AudioClip kickSound;
    public AudioSource audioSource;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;    // Bán kính đi tuần quanh vị trí ban đầu
    public float waitTimeAtPoint = 2f;  // Thời gian đứng chờ trước khi đi tiếp

    [Header("Follow / Return  Settings")]
    public float chaseRange = 8f;       // Khoảng cách phát hiện player
    public float returnRange = 15f;     // Đi quá xa vị trí ban đầu thì quay về
    public float attackDuration = 1.2f;    // Thời gian thực hiện 1 đòn tấn công

    [Header("Attack Settings")]
    public float attackRange = 2f;     // Khoảng cách tấn công
    public float attackCooldown = 2f;  // Thời gian giữa các lần tấn công

    [Header("Jump Smash Skill")]
    public GameObject jumpImpactVFX;
    public GameObject warningZonePrefab;
    public Transform jumpImpactPoint;
    public float jumpHeight = 3f;
    public float jumpDuration = 1.2f;
    public float jumpSkillRange = 6f;
    public float warningTime = 1.2f;
    public float jumpCooldown = 9f;
    public float jumpDamageRadius = 4f;
    public int jumpDamage = 80;
    public float jumpKnockBackForce = 6f;
  
    [Header("Shield Settings")]
    public GameObject shieldPrefab;
    public float shieldDuration = 5f;      // Thời gian tồn tại của shield
    public bool isShieldActive;
    public float slowDuration = 2f;       // Thời gian làm chậm player khi tấn công trúng lá chắn
    public float slowPercent = 0.5f;       // Phần trăm giảm tốc của lá chắn

    private Coroutine shieldCoroutine;        // Tham chiếu đến coroutine để có thể dừng khi cần
    private GameObject currentShield;        // Tham chiếu đến shield hiện tại nếu đang tồn tại

    private bool canJumpSkill = true;

    private float attackTimer;         // Bộ đếm thời gian giữa các lần tấn công
    private bool isAttacking;        // Trạng thái tấn công
    private float attackStateTimer;    // Bộ đếm thời gian tấn công

    private Boss02State currentState = Boss02State.Patrolling;  // Trạng thái hiện tại
    private Vector3 spawnpoint;                             // Vị trí boss sinh ra ban đầu
    private Vector3 patrolTarget;                           // Điểm đi tuần hiện tại
    private float waitTimer;                               // Bộ đếm thời gian chờ
    private bool isDead = false;                                  // Trạng thái đã chết    
    bool isJumpingSkill;   
    void Start()
    {       
        // Lưu vị trí ban đầu để làm mốc đi tuần & quay về
        spawnpoint = transform.position;
        // Chọn điểm đi tuần đầu tiên
        SetNewPatrolPoint();

        weaponHitBox.enabled = false;
        kickHitBox.enabled = false;

        bossNavMeshAgent.stoppingDistance = attackRange;     
        bossNavMeshAgent.angularSpeed = 360f;
        bossNavMeshAgent.autoBraking = true;
    }
    void Update()
    {
        if(isDead) return;
        if (playerTargetTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTargetTransform.position);
        float distanceToHome = Vector3.Distance(transform.position, spawnpoint);

        float speed = bossNavMeshAgent.velocity.magnitude;
        if(!isJumpingSkill)
        {
            bossAnimator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        }
        else
        {
            bossAnimator.SetFloat("Speed", 0);
        }

            attackTimer += Time.deltaTime;             
        switch (currentState)
        {
            case Boss02State.Patrolling:
                HandlePatrol(distanceToPlayer);
                break;

            case Boss02State.FollowingPlayer:
                HandleFollow(distanceToPlayer, distanceToHome);
                break;

            case Boss02State.Attacking:
                HandleAttack(distanceToPlayer);
                break;

            case Boss02State.Returning:
                HandleReturning();
                break;
        }
    }

    // PATROLING
    void HandlePatrol(float distanceToPlayer)
    {
        //Nếu phát hiện player trong phạm vi → chuyển trạng thái theo đuổi
        if (distanceToPlayer <= chaseRange)
        {
            currentState = Boss02State.FollowingPlayer;
            return;
        }
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
        if (!isAttacking)
        {
            //Ưu tiên Jump nếu player trong jump range
            if (!isJumpingSkill && canJumpSkill && distanceToPlayer <= jumpSkillRange)
            {
                StartAttack();
                return;
            }
            //Nếu không thì đánh thường
            if (distanceToPlayer <= attackRange && attackTimer >= attackCooldown)
            {
                StartAttack();               
                return;
            }            
        }
        // Nếu boss bị dụ đi quá xa → quay về
        if (distanceToHome > returnRange)
        {
            currentState = Boss02State.Returning;
            return;
        }
        // Nếu player chạy mất → quay lại đi tuần
        if (distanceToPlayer > chaseRange + 2f)
        {
            currentState = Boss02State.Patrolling;
            SetNewPatrolPoint();
            return;
        }
        // Di chuyển liên tục về phía player
        if(!bossNavMeshAgent.hasPath || Vector3.Distance(bossNavMeshAgent.destination, playerTargetTransform.position) > 0.5f)
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
        bossAnimator.SetFloat("Speed",0);
        bossNavMeshAgent.updateRotation = false;
        
        Vector3 lookDir = playerTargetTransform.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTargetTransform.position);

        //Ưu tiên JumpSmashSkill
        if (canJumpSkill && distanceToPlayer <= jumpSkillRange)
        {
            StartCoroutine(JumpSmashSkill());
        }
        else
        {
            int attackTyp = Random.Range(0, 2);
            if (attackTyp == 0)
            {
                bossAnimator.SetBool("isAttacking", true);
            }
            else
            {
                bossAnimator.SetBool("isJumpKick", true);
            }          
        }
        currentState = Boss02State.Attacking;
    }

    // ATTACKING
    void HandleAttack(float distanceToPlayer)
    {
        if(isJumpingSkill) return;
        attackStateTimer += Time.deltaTime;
        if(distanceToPlayer > attackRange + 1.5f)
        {
            EndAttack();
            return;
        }

        Vector3 lookDir = playerTargetTransform.position - transform.position;
        lookDir.y = 0;
      
        if(lookDir != Vector3.zero)
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
        bossAnimator.SetBool("isJumpKick",false);

        weaponHitBox.enabled = false;
        kickHitBox.enabled = false;

        bossNavMeshAgent.updateRotation = true;
        bossNavMeshAgent.updatePosition = true;
        bossNavMeshAgent.isStopped = false;

        currentState = Boss02State.FollowingPlayer;
    }
    // RETURNING
    void HandleReturning()
    {
        // Quay về vị trí ban đầu
        bossNavMeshAgent.SetDestination(spawnpoint);
        // Nếu đã về tới nơi → tiếp tục đi tuần
        if (!bossNavMeshAgent.pathPending && bossNavMeshAgent.remainingDistance <= bossNavMeshAgent.stoppingDistance)
        {
            currentState = Boss02State.Patrolling;
            SetNewPatrolPoint();
        }
    }
    //Add Animation Event
    public void EnableWeaponHitBox()
    {
        weaponHitBox.enabled = true;
    }
    public void DisableWeaponHitBox()
    {
        weaponHitBox.enabled = false;
    }
    public void EnableKickHitBox()
    {
        kickHitBox.enabled = true;
        BossKickHitbox hitbox = kickHitBox.GetComponent<BossKickHitbox>();
        if (hitbox != null)
        {
            hitbox.ResetHit();
        }
    }
    public void DisableKickHitBox()
    {
        kickHitBox.enabled = false;
    }
    public void InternalActivateShield()
    {
        if (shieldPrefab == null) return;
        if(isShieldActive) return;

        isShieldActive = true;

        currentShield = Instantiate(shieldPrefab, transform);
        currentShield.transform.localPosition = Vector3.zero;
        currentShield.transform.localRotation = Quaternion.identity;

        shieldCoroutine = StartCoroutine(ShieldDurationCoroutine());
        Debug.Log(name + " Shield Activated");      
    }
    //Logic skill JumpSmash
    IEnumerator JumpSmashSkill()
    {       
        isJumpingSkill = true;
        canJumpSkill = false;
     
        // Lấy vị trí sau khi đã stop agent
        Vector3 targetPosition = playerTargetTransform.position;
        Vector3 startPos = transform.position;

        // TẮT NavMesh position control để tránh giật lùi
        bossNavMeshAgent.updatePosition = false;
        bossNavMeshAgent.updateRotation = false;

        //Spawn vùng đỏ
        GameObject warning = Instantiate(warningZonePrefab, targetPosition, Quaternion.identity);
        
        bossAnimator.SetBool("isLanding", false);
        bossAnimator.SetBool("isJumping", true);

        float time = 0f;
        while (time < jumpDuration)
        {
            float t = time / jumpDuration;
            //di chuyển ngang
            Vector3 pos = Vector3.Lerp(startPos, targetPosition, t);

            //tạo parabol
            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = pos;
            time += Time.deltaTime;
            yield return null;
        }
        //Đảm bảo nhảy đúng vị trí
        transform.position = targetPosition;
        // ép boss chạm đất
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f))
        {
            transform.position = hit.point;
        }
        //Animation đáp đất

        bossAnimator.SetBool("isLanding", true);
        //Thêm âm thanh khi đáp đất
        if (audioSource != null && jumpLandSound != null)
        {
            audioSource.PlayOneShot(jumpLandSound);
        }
        yield return new WaitForSeconds(0.4f);
        SpawnJumpImpactVFX();
        DealJumpDamage();

        //Xoá vùng đỏ khi đáp đất
        if (warning != null)
        {
            Destroy(warning);
        }
        yield return new WaitForSeconds(0.8f);

        bossAnimator.SetBool("isJumping", false);
        bossAnimator.SetBool("isLanding", false);

        isJumpingSkill = false;

        bossNavMeshAgent.Warp(transform.position);
        bossNavMeshAgent.updatePosition = true;
        bossNavMeshAgent.updateRotation = true;
        
        EndAttack();

        //Damage sẽ đc gọi từ Animation event
        yield return new WaitForSeconds(jumpCooldown);
        canJumpSkill = true;       
    }
    //Hàm này gọi ở hàm JumpSmashSkill() để trừ máu player
    void DealJumpDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, jumpDamageRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(jumpDamage);
                }
                //  KnockBack 
                
                PlayerMovement players = hit.GetComponent<PlayerMovement>();
                if (players != null)
                {
                    Vector3 direction = hit.transform.position - transform.position;
                    direction.y = 0;
                    direction.Normalize();
                    // thêm lực hất lên
                    Vector3 force = direction * jumpKnockBackForce + Vector3.up * 2f;

                    players.ApplyKnockBack(force);
                }
            }
        }
    }
    
    //Spawn ImpactVFX
    void SpawnJumpImpactVFX()
    {
        Vector3 spawnPos = jumpImpactPoint != null ? jumpImpactPoint.position : transform.position;

        Instantiate(jumpImpactVFX, spawnPos, Quaternion.identity);
    }
    IEnumerator ShieldDurationCoroutine()
    {
        yield return new WaitForSeconds(shieldDuration);
        ForceDeactivateShield();
    }

    public void ForceDeactivateShield()
    {
        if(!isShieldActive) return;

        isShieldActive = false;

        if(shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }

        if (currentShield != null)
        {
            Destroy(currentShield);
            currentShield = null;
        }
        Boss01.ResetGlobalShield();
        Debug.Log(name + " Shield Force Deactivated");
        
    }
    public bool IsShieldActive()
    {
        return isShieldActive;
    }
    //Hàm này để gọi animation GetHit từ script health khi boss bị đánh trúng
    public void PlayerGetHit()
    {
        if(isAttacking || isJumpingSkill) return; // Không cho bị hit khi đang tấn công hoặc đang dùng skill
        if (isDead) return; //chặn animation GetHit nếu đã chết
            
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("GetHit");
        }         
    }
    //Hàm làm cho boss bị knockback khi dính đamege
    public void KnockBack(Vector3 attackerPostion, float force)
    {
        if(isDead) return;
        Vector3 direction = (transform.position - attackerPostion).normalized;
        transform.position += direction * force;
    }  
    //Hàm add âm thanh vào animation event Attack
    public void PlayOnePunchSound()
    {
        if (audioSource != null && punchSound != null)
        {
            audioSource.PlayOneShot(punchSound);
        }
    }
    //Hàm add âm thanh vào animation event Attack02 jumpKick
    public void PlayOneKickSound()
    {
        if (audioSource != null && kickSound != null)
        {
            audioSource.PlayOneShot(kickSound);
        }
    }
    // Hàm làm cho Boss die
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Boss Đie");
        StopAllCoroutines();
        if (bossNavMeshAgent != null)
        {
            bossNavMeshAgent.isStopped = true;
            bossNavMeshAgent.enabled = false;
        }

        //Xoá lá chắn nếu còn tồn tại
        if (currentShield != null) Destroy(currentShield);
        //Phát hiệu ứng chết
        bossAnimator.SetBool("isAttacking", false);
        bossAnimator.SetBool("isDead", true);

        Destroy(gameObject, 5f);
    }
    // ====== DEBUG ======
    void OnDrawGizmosSelected()
    {
        //Bán kính đi tuần
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        //Khoảng cách phát hiện người chơi
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        //Khoảng cách trở về vị trí ban đầu
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, returnRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, jumpSkillRange);
    }
}
