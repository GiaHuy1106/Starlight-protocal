using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum Enemy3State
{
    Idle, //dừng nghỉ 3s 
    Patrolling,
    FollowingPlayer,
    ReturningHome,
    Attacking
}
public class Enemy3 : MonoBehaviour
{
    public Transform playerTargetTransform;
    public NavMeshAgent slime3NavMeshAgent;

    //Trạng thái hiện tại của enemy3
    public Enemy3State currentState = Enemy3State.Patrolling;
    //Danh sách các điểm để đi tuần
    public Transform[] patrolPoints;
    //Chỉ số điểm đi tuần hiện tại
    public int currentPatrolIndex = 0;
    //vị trí ban đầu của enemy3
    public Vector3 initialPosition;
    //khoảng cách tối thiểu để enemy3 đuổi theo người chơi
    public float chaseRange = 25f;
    //khoảng cách để enemy3 quay về chỗ ban đầu
    public float returnRange = 35f;
    //animator
    public Animator enemy3Animator;
    public int enemy3SpeedHash;
    // Biến đếm thời gian nghỉ
    public float waitTimeAtPoint = 3f; // Thời gian nghỉ mong muốn
    private float currentWaitTimer = 0f; // Bộ đếm thời gian thực tế
    //Tấn công
    public float attackRange = 10f; // khoảng cách tấn công
    public float attackRate = 2.5f; // thời gian giữa các lần tấn công
    private float nextAttackTime = 0f;

    //Thêm thông số  
    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f; //tốc độ bình thường
    public float chaseSpeed = 6f; //tốc độ đuổi theo 
    [Header("Combat Settings")]
    public float rotationSpeed = 10f; // Tốc độ xoay khi tấn công
    [Header("Combat Stats")]
    public float maxHP = 300f;
    private float currentHP;
    public float damage = 30f;
    [Header("Circle Attack Settings")] 
    public GameObject magicCirclePrefab;

    private void Start()
    {
        //vị trí ban đầu
        initialPosition = transform.position;
        //khởi tạo hash cho các trạng thái animation
        enemy3SpeedHash = Enemy_Constant.Enemy3SpeedHash;
        //khởi tạo tốc độ ban đầu
        slime3NavMeshAgent.speed = patrolSpeed;
        // Đảm bảo NavMesh di chuyển tới điểm đầu tiên ngay khi vào game
        if (patrolPoints.Length > 0)
        {
            slime3NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        //khởi tạo máu
        currentHP = maxHP;

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
            case Enemy3State.Idle:
                HandleIdle(distanceToPlayer);
                break;
            case Enemy3State.Patrolling:
                HandlePatrolling(distanceToPlayer);
                break;
            case Enemy3State.FollowingPlayer:
                HandleChasing(distanceToPlayer, distanceToHome);
                break;
            case Enemy3State.ReturningHome:
                HandleReturningHome();
                break;
            case Enemy3State.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
            default:
                break;
        }
        //cập nhật trạng thái animator
        enemy3Animator.SetFloat(enemy3SpeedHash, slime3NavMeshAgent.velocity.magnitude);
    }
    void HandleIdle(float distanceToPlayer)
    {
        // Nếu người chơi đến gần trong lúc đang nghỉ thì phải đuổi theo ngay
        if (distanceToPlayer < chaseRange)
        {
            currentState = Enemy3State.FollowingPlayer;
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
            slime3NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);

            // Chuyển lại trạng thái đi tuần
            currentState = Enemy3State.Patrolling;
        }
    }
    void HandlePatrolling(float distanceToPlayer)
    {
        slime3NavMeshAgent.speed = patrolSpeed;
        //nếu người chơi đến gần, chuyển sang trạng thái theo người chơi
        if (distanceToPlayer < chaseRange)
        {
            currentState = Enemy3State.FollowingPlayer;
            return;
        }
        //di chuyển giữa các điểm đi tuần
        slime3NavMeshAgent.SetDestination(
            patrolPoints[currentPatrolIndex].position);
        //nếu điểm đi tuần hiện tại, chuyển sang điểm tiếp theo
        if (!slime3NavMeshAgent.pathPending &&
            slime3NavMeshAgent.remainingDistance < 10f)
        {
            //Chuyển sang trạng thái đợi 3s trước khi chuyển điểm
            currentState = Enemy3State.Idle;
            currentWaitTimer = 0f; // Bắt đầu đếm giờ từ 0
        }
    }
    //Xử lý hành vi đuổi theo
    void HandleChasing(float distanceToPlayer, float distanceToHome)
    {
        slime3NavMeshAgent.speed = chaseSpeed;
        //Nếu người chơi quá xa, chuyển sang trạng thái quay về chỗ ban đầu
        if (distanceToHome > returnRange)
        {
            currentState = Enemy3State.ReturningHome;
            return;
        }
        //di chuyển về phía người chơi
        slime3NavMeshAgent.SetDestination(playerTargetTransform.position);
        //nếu đến gần người chơi thì chuyển sang trạng thái tấn công
        if (distanceToPlayer < attackRange)
        {
            currentState = Enemy3State.Attacking;
            return;
        }
        //nếu người chơi đi quá xa, chuyển về trạng thái đi tuần
        if (distanceToPlayer > chaseRange + 4f)
        {
            currentState = Enemy3State.Patrolling;
            return;
        }
    }
    //xử lý quay về
    void HandleReturningHome()
    {
        slime3NavMeshAgent.speed = patrolSpeed;
        //quay về vị trí ban đầu
        slime3NavMeshAgent.SetDestination(initialPosition);
        //nếu đã về chỗ ban đầu , chuyển sang trạng thái đi tuần
        if (!slime3NavMeshAgent.pathPending &&
            slime3NavMeshAgent.remainingDistance < 10f)
        {
            currentState = Enemy3State.Patrolling;
            return;
        }
    }
    void HandleAttacking(float distanceToPlayer)
    {
        //dừng lại và tấn công 
        slime3NavMeshAgent.SetDestination(transform.position);
        //xoay mặt về phía player khi tấn công 
        RotateTowardsPlayer();
        //nếu người chơi di chuyển ra xa thì chuyển sang trạng thái đuổi theo
        if (distanceToPlayer > attackRange)
        {
            currentState = Enemy3State.FollowingPlayer;
            return;
        }
        //thực hiện tấn công
        if (Time.time >= nextAttackTime)
        {
            enemy3Animator.SetTrigger(Enemy_Constant.Enemy3AttackHash);
            CastMagicCircle();
            nextAttackTime = Time.time + attackRate;
            Debug.Log("Enemy2 attack...");
        }
    }
    void CastMagicCircle()
    {
        if (magicCirclePrefab != null)
        {
            // 1. Xác định vị trí dưới chân Player
            Vector3 spawnPosition = playerTargetTransform.position;

            // 2. Đảm bảo vòng tròn nằm sát mặt đất (y = 0 hoặc cao hơn 1 xíu để không bị chìm)
            spawnPosition.y = 0.05f;

            // 3. Sinh ra vòng tròn
            GameObject circle = Instantiate(magicCirclePrefab, spawnPosition, Quaternion.identity);

            // 4. Gán sát thương của Enemy cho cái vòng (để quản lý stat tập trung)
            MagicCircle magicScript = circle.GetComponent<MagicCircle>();
            if (magicScript != null)
            {
                magicScript.damage = this.damage;
            }
        }
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
        enemy3Animator.SetTrigger(Enemy_Constant.Enemy3GetHurtHash);
        Debug.Log("Enemy bị đánh! HP còn " + currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        //Ngừng di chuyển
        slime3NavMeshAgent.isStopped = true;
        slime3NavMeshAgent.velocity = Vector3.zero;

        //chạy anim
        enemy3Animator.SetTrigger(Enemy_Constant.Enemy3DieHash);

        //Biến mất
        StartCoroutine(WaitAndDisable(3f));

    }
    IEnumerator WaitAndDisable(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        gameObject.SetActive(false);
    }
    
}
