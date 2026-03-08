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

    [Header("Shield Settings")]
    public GameObject shieldPrefab;
    public float shieldDuration = 5f;      // Thời gian tồn tại của shield
    public bool isShieldActive;

    private Coroutine shieldCoroutine;        // Tham chiếu đến coroutine để có thể dừng khi cần
    private GameObject currentShield;        // Tham chiếu đến shield hiện tại nếu đang tồn tại
    

    private float attackTimer;         // Bộ đếm thời gian giữa các lần tấn công
    private bool isAttacking;        // Trạng thái tấn công
    private float attackStateTimer;    // Bộ đếm thời gian tấn công

    private Boss02State currentState = Boss02State.Patrolling;  // Trạng thái hiện tại
    private Vector3 spawnpoint;                             // Vị trí boss sinh ra ban đầu
    private Vector3 patrolTarget;                           // Điểm đi tuần hiện tại
    private float waitTimer;                               // Bộ đếm thời gian chờ
    private bool isDead = false;                                  // Trạng thái đã chết
    void Start()
    {
        //bossNavMeshAgent.SetDestination(playerTargetTransform.position);
        //bossSpeedHash = Constants.speedBoss;

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
        bossAnimator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

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
        if (!isAttacking && distanceToPlayer <= attackRange + 0.1f && attackTimer >= attackCooldown)
        {
            StartAttack();
            //currentState = BossState.Attacking;
            return;
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
        bossNavMeshAgent.updateRotation = false;
        //bossNavMeshAgent.updatePosition = false;

        Vector3 lookDir = playerTargetTransform.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        int attackTyp = Random.Range(0, 2);
        if(attackTyp ==0)
        {
            bossAnimator.SetBool("isAttacking", true);
        }
        else
        {
            bossAnimator.SetTrigger("JumpKick");
        }

        currentState = Boss02State.Attacking;
    }
    // ATTACKING
    void HandleAttack(float distanceToPlayer)
    {
        attackStateTimer += Time.deltaTime;
        if(distanceToPlayer > attackRange + 1.5f)
        {
            EndAttack();
            return;
        }

        Vector3 lookDir = playerTargetTransform.position - transform.position;
        lookDir.y = 0;

        //Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        //transform.rotation = Quaternion.LookRotation(lookDir);
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
        weaponHitBox.enabled = false;
        kickHitBox.enabled = false;

        bossNavMeshAgent.updateRotation = true;
        //bossNavMeshAgent.updatePosition = true;
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
        if(isAttacking) return; //chặn animation GetHit nếu đang tấn công
        if(isDead) return; //chặn animation GetHit nếu đã chết
        if(bossAnimator != null)
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
    }
}
