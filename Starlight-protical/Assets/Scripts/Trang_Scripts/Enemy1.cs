using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum Enemy1State
{
    Idle, //dừng nghỉ 3s 
    Patrolling,
    FollowingPlayer,
    ReturningHome,
    Attacking
}
public class Enemy1 : MonoBehaviour
{
    public Transform playerTargetTransform;
    public NavMeshAgent slime1NavMeshAgent;
       
    //Trạng thái hiện tại của enemy1
    public Enemy1State currentState = Enemy1State.Patrolling;
    //Danh sách các điểm để đi tuần
    public Transform[] patrolPoints;
    //Chỉ số điểm đi tuần hiện tại
    public int currentPatrolIndex = 0;
    //vị trí ban đầu của enemy1
    public Vector3 initialPosition;
    //khoảng cách tối thiểu để enemy1 đuổi theo người chơi
    public float chaseRange = 10f;
    //khoảng cách để enemy1 quay về chỗ ban đầu
    public float returnRange = 20f;
    //animator
    public Animator enemy1Animator;
    public int enemy1SpeedHash;
    // Biến đếm thời gian nghỉ
    public float waitTimeAtPoint = 3f; // Thời gian nghỉ mong muốn
    private float currentWaitTimer = 0f; // Bộ đếm thời gian thực tế
    //Tấn công
    public float attackRange = 2f; // khoảng cách tấn công
    public float attackRate = 1.5f; // thời gian giữa các lần tấn công
    private float nextAttackTime = 0f;

    //Thêm thông số  
    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f; //tốc độ bình thường
    public float chaseSpeed = 6f; //tốc độ đuổi theo 
    [Header("Combat Settings")]
    public float rotationSpeed = 10f; // Tốc độ xoay khi tấn công
    [Header("Combat Stats")]
    public float maxHP = 100f;
    private float currentHP;
    public float damage = 10f;
    private void Start()
    {
        //vị trí ban đầu
        initialPosition = transform.position;
        //khởi tạo hash cho các trạng thái animation
        enemy1SpeedHash = Enemy_Constant.Enemy1SpeedHash;
        //khởi tạo tốc độ ban đầu
        slime1NavMeshAgent.speed = patrolSpeed;
        // Đảm bảo NavMesh di chuyển tới điểm đầu tiên ngay khi vào game
        if (patrolPoints.Length > 0)
        {
            slime1NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
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
            initialPosition );
        //xử lý hành vi dựa trên trạng thái hiện tại
        switch(currentState)
        {
            case Enemy1State.Idle:
                HandleIdle(distanceToPlayer); 
                break;
            case Enemy1State.Patrolling:
                HandlePatrolling( distanceToPlayer ); 
                break;
            case Enemy1State.FollowingPlayer:
                HandleChasing(distanceToPlayer, distanceToHome); 
                break;
            case Enemy1State.ReturningHome:
                HandleReturningHome();
                break;
            case Enemy1State.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
            default:
                break;
        }
        //cập nhật trạng thái animator
        enemy1Animator.SetFloat(enemy1SpeedHash, slime1NavMeshAgent.velocity.magnitude);
    }
    void HandleIdle(float distanceToPlayer)
    {
        // Nếu người chơi đến gần trong lúc đang nghỉ thì phải đuổi theo ngay
        if (distanceToPlayer < chaseRange)
        {
            currentState = Enemy1State.FollowingPlayer;
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
            slime1NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);

            // Chuyển lại trạng thái đi tuần
            currentState = Enemy1State.Patrolling;
        }
    }    
    void HandlePatrolling(float distanceToPlayer)
    {
        slime1NavMeshAgent.speed = patrolSpeed;
        //nếu người chơi đến gần, chuyển sang trạng thái theo người chơi
        if(distanceToPlayer < chaseRange)
        {
            currentState = Enemy1State.FollowingPlayer;
            return;
        }
        //di chuyển giữa các điểm đi tuần
        slime1NavMeshAgent.SetDestination(
            patrolPoints[currentPatrolIndex].position);
        //nếu điểm đi tuần hiện tại, chuyển sang điểm tiếp theo
        if(!slime1NavMeshAgent.pathPending &&
            slime1NavMeshAgent.remainingDistance<2f)
        {
            //Chuyển sang trạng thái đợi 3s trước khi chuyển điểm
            currentState = Enemy1State.Idle;
            currentWaitTimer = 0f; // Bắt đầu đếm giờ từ 0
        }    
    }    
    //Xử lý hành vi đuổi theo
    void HandleChasing(float distanceToPlayer, float distanceToHome)
    {
        slime1NavMeshAgent.speed = chaseSpeed;
        //Nếu người chơi quá xa, chuyển sang trạng thái quay về chỗ ban đầu
        if(distanceToHome > returnRange)
        {
            currentState = Enemy1State.ReturningHome;
            return;
        }    
        //di chuyển về phía người chơi
        slime1NavMeshAgent.SetDestination(playerTargetTransform.position);
        //nếu đến gần người chơi thì chuyển sang trạng thái tấn công
        if(distanceToPlayer < attackRange)
        {
            currentState = Enemy1State.Attacking;
            return;
        }    
        //nếu người chơi đi quá xa, chuyển về trạng thái đi tuần
        if(distanceToPlayer>chaseRange +2f)
        {
            currentState = Enemy1State.Patrolling;
            return;
        }    
    }    
    //xử lý quay về
    void HandleReturningHome()
    {
        slime1NavMeshAgent.speed = patrolSpeed;
        //quay về vị trí ban đầu
        slime1NavMeshAgent.SetDestination(initialPosition);
        //nếu đã về chỗ ban đầu , chuyển sang trạng thái đi tuần
        if(!slime1NavMeshAgent.pathPending &&
            slime1NavMeshAgent.remainingDistance<2f)
        {
            currentState = Enemy1State.Patrolling;
            return;
        }    
    }    
    void HandleAttacking(float distanceToPlayer)
    {
        //dừng lại và tấn công 
        slime1NavMeshAgent.SetDestination(transform.position);
        //xoay mặt về phía player khi tấn công 
        RotateTowardsPlayer();
        //nếu người chơi di chuyển ra xa thì chuyển sang trạng thái đuổi theo
        if (distanceToPlayer > attackRange)
        {
            currentState = Enemy1State.FollowingPlayer;
            return;
        }    
        //thực hiện tấn công
        if(Time.time >= nextAttackTime)
        {
            enemy1Animator.SetTrigger(Enemy_Constant.Enemy1AttackHash);
            playerTargetTransform.GetComponent<FakePlayerHealth>().TakeDamage(damage);
            nextAttackTime = Time.time + attackRate;
            Debug.Log("Enemy1 attack...");
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
        enemy1Animator.SetTrigger(Enemy_Constant.Enemy1GetHurtHash);
        Debug.Log("Enemy bị đánh! HP còn " +  currentHP);
        if(currentHP <= 0)
        {
            Die();
        }    
    }    
    void Die()
    {
        //Ngừng di chuyển
        slime1NavMeshAgent.isStopped = true;
        slime1NavMeshAgent.velocity = Vector3.zero;

        //chạy anim
        enemy1Animator.SetTrigger(Enemy_Constant.Enemy1DieHash);

        //Biến mất
        StartCoroutine(WaitAndDisable(2f));

    }
    IEnumerator WaitAndDisable(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        gameObject.SetActive(false);
    }
}
