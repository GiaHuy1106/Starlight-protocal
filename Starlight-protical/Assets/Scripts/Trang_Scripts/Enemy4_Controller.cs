using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum Enemy4State
{
    Idle, //dừng nghỉ 3s 
    Patrolling,
    FollowingPlayer,
    ReturningHome,
    Attacking
}
public class Enemy4 : MonoBehaviour
{
    public Transform playerTargetTransform;
    public NavMeshAgent slime4NavMeshAgent;

    //Trạng thái hiện tại của enemy4
    public Enemy4State currentState = Enemy4State.Patrolling;
    //Danh sách các điểm để đi tuần
    public Transform[] patrolPoints;
    //Chỉ số điểm đi tuần hiện tại
    public int currentPatrolIndex = 0;
    //vị trí ban đầu của enemy4
    public Vector3 initialPosition;
    //khoảng cách tối thiểu để enemy4 đuổi theo người chơi
    public float chaseRange = 30f;
    //khoảng cách để enemy4 quay về chỗ ban đầu
    public float returnRange = 40f;
    //animator
    public Animator enemy4Animator;
    public int enemy4SpeedHash;
    // Biến đếm thời gian nghỉ
    public float waitTimeAtPoint = 3f; // Thời gian nghỉ mong muốn
    private float currentWaitTimer = 0f; // Bộ đếm thời gian thực tế
    //Tấn công
    public float attackRange = 15f; // khoảng cách tấn công
    public float attackRate = 13f; // thời gian giữa các lần tấn công
    private float nextAttackTime = 0f;

    //Thêm thông số  
    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f; //tốc độ bình thường
    public float chaseSpeed = 8f; //tốc độ đuổi theo 
    [Header("Combat Settings")]
    public float rotationSpeed = 10f; // Tốc độ xoay khi tấn công
    [Header("Combat Stats")]
    public float maxHP = 400f;
    private float currentHP;

    public GameObject PoisonArea;
    public float poisonDuration = 10f; // Vòng độc hiện 10s rồi tắt
    private void Start()
    {
        //vị trí ban đầu
        initialPosition = transform.position;
        //khởi tạo hash cho các trạng thái animation
        enemy4SpeedHash = Enemy_Constant.Enemy4SpeedHash;
        //khởi tạo tốc độ ban đầu
        slime4NavMeshAgent.speed = patrolSpeed;
        // Đảm bảo NavMesh di chuyển tới điểm đầu tiên ngay khi vào game
        if (patrolPoints.Length > 0)
        {
            slime4NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        //khởi tạo máu
        currentHP = maxHP;
        //Đảm bảo tắt vòng độc 
        if (PoisonArea != null) PoisonArea.SetActive(false);
        //khởi tạo stopping distance 
        slime4NavMeshAgent.stoppingDistance = 0f;
    }
    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(
            transform.position,
            playerTargetTransform.position);
        var distanceToHome = Vector3.Distance(
            transform.position,
            initialPosition);
        //xử lý hành vi dựa trên trạng thái hiện tại
        switch (currentState)
        {
            case Enemy4State.Idle:
                HandleIdle(distanceToPlayer);
                break;
            case Enemy4State.Patrolling:
                HandlePatrolling(distanceToPlayer);
                break;
            case Enemy4State.FollowingPlayer:
                HandleChasing(distanceToPlayer, distanceToHome);
                break;
            case Enemy4State.ReturningHome:
                HandleReturningHome();
                break;
            case Enemy4State.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
            default:
                break;
        }
        //cập nhật trạng thái animator
        enemy4Animator.SetFloat(enemy4SpeedHash, slime4NavMeshAgent.velocity.magnitude);
    }
    void HandleIdle(float distanceToPlayer)
    {
        slime4NavMeshAgent.stoppingDistance = 0f;
        // Nếu người chơi đến gần trong lúc đang nghỉ thì phải đuổi theo ngay
        if (distanceToPlayer < chaseRange)
        {
            currentState = Enemy4State.FollowingPlayer;
            currentWaitTimer = 0; // Reset timer
            return;
        }

        // 2. Tính giờ nghỉ
        currentWaitTimer += Time.deltaTime;

        // 3. Nếu đã nghỉ đủ 3 giây
        if (currentWaitTimer >= waitTimeAtPoint)
        {
            // Chuyển sang điểm tiếp theo
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

            // Set đích đến mới
            slime4NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);

            // Chuyển lại trạng thái đi tuần
            currentState = Enemy4State.Patrolling;
        }
    }
    void HandlePatrolling(float distanceToPlayer)
    {
        slime4NavMeshAgent.stoppingDistance = 0f;
        slime4NavMeshAgent.speed = patrolSpeed;
        //nếu người chơi đến gần, chuyển sang trạng thái theo người chơi
        if (distanceToPlayer < chaseRange)
        {
            currentState = Enemy4State.FollowingPlayer;
            return;
        }
        //di chuyển giữa các điểm đi tuần
        slime4NavMeshAgent.SetDestination(
            patrolPoints[currentPatrolIndex].position);
        //nếu điểm đi tuần hiện tại, chuyển sang điểm tiếp theo
        if (!slime4NavMeshAgent.pathPending &&
            slime4NavMeshAgent.remainingDistance < 2f)
        {
            //Chuyển sang trạng thái đợi 3s trước khi chuyển điểm
            currentState = Enemy4State.Idle;
            currentWaitTimer = 0f; // Bắt đầu đếm giờ từ 0
        }
    }
    //Xử lý hành vi đuổi theo
    void HandleChasing(float distanceToPlayer, float distanceToHome)
    {
        slime4NavMeshAgent.stoppingDistance = 8f;
        slime4NavMeshAgent.speed = chaseSpeed;
        //Nếu người chơi quá xa, chuyển sang trạng thái quay về chỗ ban đầu
        if (distanceToHome > returnRange)
        {
            currentState = Enemy4State.ReturningHome;
            return;
        }
        //di chuyển về phía người chơi
        slime4NavMeshAgent.SetDestination(playerTargetTransform.position);
        //nếu đến gần người chơi thì chuyển sang trạng thái tấn công
        if (distanceToPlayer < attackRange)
        {
            
            currentState = Enemy4State.Attacking;
            return;
        }
        //nếu người chơi đi quá xa, chuyển về trạng thái đi tuần
        if (distanceToPlayer > chaseRange + 1f)
        {
            currentState = Enemy4State.Patrolling;
            return;
        }
    }
    //xử lý quay về
    void HandleReturningHome()
    {
        slime4NavMeshAgent.stoppingDistance = 0f;
        slime4NavMeshAgent.speed = patrolSpeed;
        //quay về vị trí ban đầu
        slime4NavMeshAgent.SetDestination(initialPosition);
        //nếu đã về chỗ ban đầu , chuyển sang trạng thái đi tuần
        if (!slime4NavMeshAgent.pathPending &&
            slime4NavMeshAgent.remainingDistance < 2f)
        {
            currentState = Enemy4State.Patrolling;
            return;
        }
    }
    void HandleAttacking(float distanceToPlayer)
    {
        slime4NavMeshAgent.stoppingDistance = 8f;
        //dừng lại và tấn công 
        slime4NavMeshAgent.SetDestination(transform.position);
        //xoay mặt về phía player khi tấn công 
        RotateTowardsPlayer();
        //nếu người chơi di chuyển ra xa thì chuyển sang trạng thái đuổi theo
        if (distanceToPlayer > attackRange)
        {
            currentState = Enemy4State.FollowingPlayer;
            return;
        }
        //thực hiện tấn công
        if (Time.time >= nextAttackTime)
        {
            enemy4Animator.SetTrigger(Enemy_Constant.Enemy4AttackHash);
            if (PoisonArea != null)
            {
                StartCoroutine(ActivatePoisonAbility());
            }
            nextAttackTime = Time.time + attackRate;
            Debug.Log("Enemy4 attack...");
        }
    }
    IEnumerator ActivatePoisonAbility()
    {
        // Bật vòng độc
        PoisonArea.SetActive(true);

        // Chờ X giây (ví dụ 3s)
        yield return new WaitForSeconds(poisonDuration);

        // Tắt vòng độc
        PoisonArea.SetActive(false);
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (playerTargetTransform.position - transform.position).normalized;
        direction.y = 0; // Khóa trục Y để enemy không bị nghiêng ngả

        if (direction != Vector3.zero) // Tránh lỗi nếu vị trí trùng nhau
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // Xoay từ từ , thay vì giật cục
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        enemy4Animator.SetTrigger(Enemy_Constant.Enemy4GetHurtHash);
        Debug.Log("Enemy bị đánh! HP còn " + currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        //Ngừng di chuyển
        slime4NavMeshAgent.isStopped = true;
        slime4NavMeshAgent.velocity = Vector3.zero;

        //chạy anim
        enemy4Animator.SetTrigger(Enemy_Constant.Enemy4DieHash);

        //Biến mất
        StartCoroutine(WaitAndDisable(3f));

    }
    IEnumerator WaitAndDisable(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        gameObject.SetActive(false);
    }

}
