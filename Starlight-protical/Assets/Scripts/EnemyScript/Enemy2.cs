using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum Enemy2State
{
    Idle, //dừng nghỉ 3s 
    Patrolling,
    FollowingPlayer,
    ReturningHome,
    Defending,
    Attacking,
    Die
}
[System.Serializable]
public class LootItem2
{
    public ItemObject itemData;
}
public class Enemy2 : MonoBehaviour
{
    public static Enemy2 Instance { get; private set;}

    public Transform playerTargetTransform;
    public NavMeshAgent slime2NavMeshAgent;

    //Trạng thái hiện tại của enemy2
    public Enemy2State currentState = Enemy2State.Patrolling;
    //Danh sách các điểm để đi tuần
    public Transform[] patrolPoints;
    //Chỉ số điểm đi tuần hiện tại
    public int currentPatrolIndex = 0;
    //vị trí ban đầu của enemy2
    public Vector3 initialPosition;
    //khoảng cách tối thiểu để enemy2 đuổi theo người chơi
    public float chaseRange = 20f;
    //khoảng cách để enemy2 quay về chỗ ban đầu
    public float returnRange = 30f;
    //animator
    public Animator enemy2Animator;
    public int enemy2SpeedHash;
    // Biến đếm thời gian nghỉ
    public float waitTimeAtPoint = 3f; // Thời gian nghỉ mong muốn
    private float currentWaitTimer = 0f; // Bộ đếm thời gian thực tế
    //Tấn công
    public float attackRange = 5f; // khoảng cách tấn công
    public float attackRate = 2f; // thời gian giữa các lần tấn công
    private float nextAttackTime = 0f;
    //Âm thanh
    public AudioSource music;
    public AudioClip[] musicClip; // thứ tự như sau 0.fight 1.patrolling 2.stop 3.hurt 4.die 5.defense
    //Thêm thông số  
    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f; //tốc độ bình thường
    public float chaseSpeed = 5f; //tốc độ đuổi theo 
    [Header("Combat Settings")]
    public float rotationSpeed = 10f; // Tốc độ xoay khi tấn công
    [Header("Combat Stats")]
    public float maxHP = 200f;
    private float currentHP;
    public float damage = 20f;
    public float def = 20f;
    [Header("Ranged Attack")]
    public GameObject bulletPrefab; // viên đạn prefab
    public Transform firePoint;     // Vị trí nòng 
    [Header("Defense Settings")]
    [Range(0, 100)]
    public float defendChance = 30f; // 30% cơ hội đỡ đòn
    public float defendDuration = 1.5f; // Thời gian đứng đỡ (giây)
    private bool isDefending = false;   // Biến cờ để khóa logic khi đang đỡ


    //khoảng cách giữa 2 lần di chuyển
    private float stepTimer = 0f;
    public float stepInterval = 0.7f;
    //lập cờ để biết nó phát âm thanh dừng chưa
    private bool hasPlayedStopSound = false;

    //hiện panel profile
    public GameObject miniProfilePanel;
    public Slider hpSlider;

    //rớt đồ 
    [Header("Loot System")]
    public List<LootItem2> lootTable = new List<LootItem2>();
    bool isDrop = false;

    public GameObject shield;

    //hiện máu bị trừ
    public TextMeshProUGUI takeDamageText;
    private bool isDead = false;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //tắt shield
        shield.SetActive(false);
        //vị trí ban đầu
        initialPosition = transform.position;
        //khởi tạo hash cho các trạng thái animation
        enemy2SpeedHash = Enemy_Constant.Enemy2SpeedHash;
        //khởi tạo tốc độ ban đầu
        slime2NavMeshAgent.speed = patrolSpeed;
        // Đảm bảo NavMesh di chuyển tới điểm đầu tiên ngay khi vào game
        if (patrolPoints.Length > 0)
        {
            slime2NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        //khởi tạo máu
        currentHP = maxHP;
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
        //khởi tạo stopping distance 
        slime2NavMeshAgent.stoppingDistance = 0f;
        //tắt panel 
        miniProfilePanel.SetActive(false);
        takeDamageText.text = null;
    }
    private void Update()
    {
        // Nếu đang bận đỡ đòn hoặc đã chết thì không tính toán AI nữa
        if (isDefending) return;
        var distanceToPlayer = Vector3.Distance(
            transform.position,
            playerTargetTransform.position);
        var distanceToHome = Vector3.Distance(
            transform.position,
            initialPosition);
        //xử lý hành vi dựa trên trạng thái hiện tại
        switch (currentState)
        {
            case Enemy2State.Idle:
                HandleIdle(distanceToPlayer);
                break;
            case Enemy2State.Patrolling:
                HandlePatrolling(distanceToPlayer);
                break;
            case Enemy2State.FollowingPlayer:
                HandleChasing(distanceToPlayer, distanceToHome);
                break;
            case Enemy2State.ReturningHome:
                HandleReturningHome();
                break;
            case Enemy2State.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
            case Enemy2State.Die:
                Die();
                break;
            default:
                break;
        }
        //cập nhật trạng thái animator
        enemy2Animator.SetFloat(enemy2SpeedHash, slime2NavMeshAgent.velocity.magnitude);
        bool isMovingState = currentState == Enemy2State.Patrolling;

        if (isMovingState &&  slime2NavMeshAgent.velocity.magnitude > 0.1f)
        {
            // Trừ dần thời gian chờ
            stepTimer -= Time.deltaTime;

            // Khi thời gian đếm ngược về 0 hoặc âm
            if (stepTimer <= 0f)
            {
                music.PlayOneShot(musicClip[1]);
                stepTimer = stepInterval; // Đặt lại bộ đếm cho bước tiếp theo
            }
            hasPlayedStopSound = false;
        }
        else
        {
            // Reset lại timer khi đứng im để lần đi tiếp theo phát tiếng ngay lập tức
            stepTimer = 0f;
            if (hasPlayedStopSound == false)
            {
                music.PlayOneShot(musicClip[2]);
                hasPlayedStopSound = true;
            }
        }
    }
    void HandleIdle(float distanceToPlayer)
    {
        slime2NavMeshAgent.stoppingDistance = 0f;
        // Nếu người chơi đến gần trong lúc đang nghỉ thì phải đuổi theo ngay
        if (distanceToPlayer < chaseRange)
        {
            currentState = Enemy2State.FollowingPlayer;
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
            slime2NavMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);

            // Chuyển lại trạng thái đi tuần
            currentState = Enemy2State.Patrolling;
        }
    }
    void HandlePatrolling(float distanceToPlayer)
    {
        slime2NavMeshAgent.stoppingDistance = 0f;
        slime2NavMeshAgent.speed = patrolSpeed;
        //nếu người chơi đến gần, chuyển sang trạng thái theo người chơi
        if (distanceToPlayer < chaseRange)
        {
            currentState = Enemy2State.FollowingPlayer;
            return;
        }
        //di chuyển giữa các điểm đi tuần
        slime2NavMeshAgent.SetDestination(
            patrolPoints[currentPatrolIndex].position);
        //nếu điểm đi tuần hiện tại, chuyển sang điểm tiếp theo
        if (!slime2NavMeshAgent.pathPending &&
            slime2NavMeshAgent.remainingDistance < 2f)
        {
            //Chuyển sang trạng thái đợi 3s trước khi chuyển điểm
            currentState = Enemy2State.Idle;
            currentWaitTimer = 0f; // Bắt đầu đếm giờ từ 0
        }
    }
    //Xử lý hành vi đuổi theo
    void HandleChasing(float distanceToPlayer, float distanceToHome)
    {
        slime2NavMeshAgent.stoppingDistance = 5f;
        slime2NavMeshAgent.speed = chaseSpeed;
        //Nếu người chơi quá xa, chuyển sang trạng thái quay về chỗ ban đầu
        if (distanceToHome > returnRange)
        {
            currentState = Enemy2State.ReturningHome;
            miniProfilePanel.SetActive(false);
            return;
        }
        //di chuyển về phía người chơi
        slime2NavMeshAgent.SetDestination(playerTargetTransform.position);
        //nếu đến gần người chơi thì chuyển sang trạng thái tấn công
        if (distanceToPlayer < attackRange)
        {
            miniProfilePanel.SetActive(true);
            currentState = Enemy2State.Attacking;
            return;
        }
        //nếu người chơi đi quá xa, chuyển về trạng thái đi tuần
        if (distanceToPlayer > chaseRange + 1f)
        {
            miniProfilePanel.SetActive(false);
            currentState = Enemy2State.Patrolling;
            return;
        }
    }
    //xử lý quay về
    void HandleReturningHome()
    {
        slime2NavMeshAgent.stoppingDistance = 0f;
        slime2NavMeshAgent.speed = patrolSpeed;
        //quay về vị trí ban đầu
        slime2NavMeshAgent.SetDestination(initialPosition);
        //nếu đã về chỗ ban đầu , chuyển sang trạng thái đi tuần
        if (!slime2NavMeshAgent.pathPending &&
            slime2NavMeshAgent.remainingDistance < 2f)
        {
            currentState = Enemy2State.Patrolling;
            return;
        }
    }
    void HandleAttacking(float distanceToPlayer)
    {
        slime2NavMeshAgent.stoppingDistance = 5f;
        //dừng lại và tấn công 
        slime2NavMeshAgent.SetDestination(transform.position);
        //xoay mặt về phía player khi tấn công 
        RotateTowardsPlayer();
        //nếu người chơi di chuyển ra xa thì chuyển sang trạng thái đuổi theo
        if (distanceToPlayer > attackRange)
        {
            currentState = Enemy2State.FollowingPlayer;
            return;
        }
        //thực hiện tấn công
        if (Time.time >= nextAttackTime)
        {
            enemy2Animator.SetTrigger(Enemy_Constant.Enemy2AttackHash);
            ShootBullet();
            nextAttackTime = Time.time + attackRate;
            Debug.Log("Enemy2 attack...");
        }
    }
    void ShootBullet()
    {
        if(bulletPrefab != null && firePoint !=null)
        {
            //tạo đạn theo hướng nòng
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            music.PlayOneShot(musicClip[0]);
            Enemy2_Bullet enemy2_bullet = bullet.GetComponent<Enemy2_Bullet>();
            if(enemy2_bullet != null)
            {
                enemy2_bullet.damage = this.damage;
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
        if (isDead) return;   // 🔴 chặn damage sau khi chết
        if (currentHP <= 0 || isDefending) return; // Chết rồi hoặc đang đỡ thì thôi
        // Tính toán xác suất đỡ đòn
        float randomValue = Random.Range(0f, 100f);

        // Nếu random ra số nhỏ hơn defendChance -> ĐỠ ĐÒN THÀNH CÔNG
        if (randomValue < defendChance)
        {
            StartCoroutine(PerformDefend());
            return; // Dừng hàm tại đây, KHÔNG TRỪ MÁU 
        }
        //trừ máu theo giáp
        if (amount > def)
        {
            amount -= def;
            StartCoroutine(TakeDamageTextAppear(amount));
            currentHP -= amount;
        }
        else
        {
            Debug.Log("Sát thương không đủ phá giáp");
        }
        hpSlider.value = currentHP;
        enemy2Animator.SetTrigger(Enemy_Constant.Enemy2GetHurtHash);
        music.PlayOneShot(musicClip[3]);
        Debug.Log("Enemy bị đánh! HP còn " + currentHP);
        if (currentHP <= 0)
        {
            currentState = Enemy2State.Die;
        }
    }
    public void Die()
    {
        
        if (isDead) return;   // nếu đã chết rồi thì thoát
        isDead = true;        // đánh dấu đã chết
        //Ngừng di chuyển
        slime2NavMeshAgent.isStopped = true;
        slime2NavMeshAgent.velocity = Vector3.zero;
        //âm thanh
        music.PlayOneShot(musicClip[4]);
        //chạy anim
        enemy2Animator.SetTrigger(Enemy_Constant.Enemy2DieHash);
        //Tắt profile
        miniProfilePanel.SetActive(false);
        //Rớt đồ
        if (!isDrop && lootTable != null && lootTable.Count > 0)
        {
            //lấy vị trí ngẫu nhiên trong danh sách lootTable
            int randomIndex = Random.Range(0, lootTable.Count);
            //random 1 trong các vật phẩm 
            LootItem2 randomLoot = lootTable[randomIndex];
            //chọn vị trí rơi 
            Vector3 dropPos = transform.position + new Vector3(1, 1, 1);
            //nếu như prefab khác null thì rơi ra 
            if (randomLoot.itemData.worldPrefab != null)
            {
                Instantiate(randomLoot.itemData.worldPrefab, dropPos, Quaternion.identity);
                isDrop = true; //đánh dấu đã rớt đồ 
            }
        }
        //Biến mất
        StartCoroutine(WaitAndDisable(3f));

    }
    IEnumerator WaitAndDisable(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        gameObject.SetActive(false);
    }
    // Coroutine xử lý hành động Đỡ đòn
    IEnumerator PerformDefend()
    {
        isDefending = true; // Khóa Update
        currentState = Enemy2State.Defending; // Chuyển state 

        // 1. Dừng di chuyển ngay lập tức
        slime2NavMeshAgent.isStopped = true;
        slime2NavMeshAgent.velocity = Vector3.zero;

        enemy2Animator.SetTrigger(Enemy_Constant.Enemy2DefendHash);
        music.PlayOneShot(musicClip[5]);
        Debug.Log("Enemy defend");
        // mở shield
        shield.SetActive(true);
        // 3. Chờ hết thời gian đỡ 
        yield return new WaitForSeconds(defendDuration);
        //tắt shield
        shield.SetActive(false);
        // 4. Quay lại trạng thái chiến đấu
        isDefending = false;
        slime2NavMeshAgent.isStopped = false; // Mở lại di chuyển

        // Sau khi đỡ xong, thường sẽ quay lại đuổi theo hoặc tấn công ngay
        currentState = Enemy2State.FollowingPlayer;
    }
    IEnumerator TakeDamageTextAppear(float amount)
    {
        takeDamageText.text = "- " + amount.ToString();
        yield return new WaitForSeconds(1f);
        takeDamageText.text = "";
    }
}
