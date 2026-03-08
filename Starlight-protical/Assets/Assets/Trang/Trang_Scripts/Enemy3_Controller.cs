using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum Enemy3State
{
    Idle, //dừng nghỉ 3s 
    Patrolling,
    FollowingPlayer,
    ReturningHome,
    Attacking,
    Die
}
[System.Serializable]
public class LootItem3
{
    public ItemObject itemData;
}
public class Enemy3_Controller : MonoBehaviour
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
    public float maxHP = 400f;
    private float currentHP;
    public float damage = 40f;
    public float def = 40f;
    [Header("Circle Attack Settings")] 
    public GameObject magicCirclePrefab;

    //Âm thanh
    public AudioSource music;
    public AudioClip[] musicClip; //thứ tự như sau: 0.fight 1.patrolling  2.Stop 3.Hurt 4.die
    //khoảng cách giữa 2 tiếng nhảy 
    private float stepTimer = 0f;
    public float stepInterval = 0.7f;
    //lập cờ để biết nó phát âm thanh dừng chưa
    private bool hasPlayedStopSound = false;

    //hiện panel profile
    public GameObject miniProfilePanel;
    public Slider hpSlider;

    //rớt đồ 
    [Header("Loot System")]
    public List<LootItem3> lootTable = new List<LootItem3>();
    bool isDrop = false;

    //hiện máu bị trừ
    public TextMeshProUGUI takeDamageText;
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
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
        //khởi tạo stopping distance 
        slime3NavMeshAgent.stoppingDistance = 0f;
        //tắt panel 
        miniProfilePanel.SetActive(false);
        takeDamageText.text = null;
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
            case Enemy3State.Die:
                Die();
                break;
            default:
                break;
        }
        //cập nhật trạng thái animator
        enemy3Animator.SetFloat(enemy3SpeedHash, slime3NavMeshAgent.velocity.magnitude);
        bool isMovingState = currentState == Enemy3State.Patrolling;

        if (isMovingState && slime3NavMeshAgent.velocity.magnitude > 0.1f)
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
        slime3NavMeshAgent.stoppingDistance = 0f;
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
        slime3NavMeshAgent.stoppingDistance = 0f;
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
            slime3NavMeshAgent.remainingDistance < 2f)
        {
            //Chuyển sang trạng thái đợi 3s trước khi chuyển điểm
            currentState = Enemy3State.Idle;
            currentWaitTimer = 0f; // Bắt đầu đếm giờ từ 0
        }
    }
    //Xử lý hành vi đuổi theo
    void HandleChasing(float distanceToPlayer, float distanceToHome)
    {
        slime3NavMeshAgent.stoppingDistance = 10f;
        slime3NavMeshAgent.speed = chaseSpeed;
        miniProfilePanel.SetActive(true);
        //Nếu người chơi quá xa, chuyển sang trạng thái quay về chỗ ban đầu
        if (distanceToHome > returnRange)
        {
            currentState = Enemy3State.ReturningHome;
            miniProfilePanel.SetActive(false);
            return;
        }
        //di chuyển về phía người chơi
        slime3NavMeshAgent.SetDestination(playerTargetTransform.position);
        //nếu đến gần người chơi thì chuyển sang trạng thái tấn công
        if (distanceToPlayer < attackRange)
        {
            miniProfilePanel.SetActive(true);
            currentState = Enemy3State.Attacking;
            return;
        }
        //nếu người chơi đi quá xa, chuyển về trạng thái đi tuần
        if (distanceToPlayer > chaseRange + 1f)
        {
            miniProfilePanel.SetActive(false);
            currentState = Enemy3State.Patrolling;
            return;
        }
    }
    //xử lý quay về
    void HandleReturningHome()
    {
        slime3NavMeshAgent.stoppingDistance = 0f;
        slime3NavMeshAgent.speed = patrolSpeed;
        //quay về vị trí ban đầu
        slime3NavMeshAgent.SetDestination(initialPosition);
        //nếu đã về chỗ ban đầu , chuyển sang trạng thái đi tuần
        if (!slime3NavMeshAgent.pathPending &&
            slime3NavMeshAgent.remainingDistance < 2f)
        {
            currentState = Enemy3State.Patrolling;
            return;
        }
    }
    void HandleAttacking(float distanceToPlayer)
    {
        slime3NavMeshAgent.stoppingDistance = 10f;
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

            music.PlayOneShot(musicClip[0]);

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
        enemy3Animator.SetTrigger(Enemy_Constant.Enemy3GetHurtHash);
        music.PlayOneShot(musicClip[3]);
        Debug.Log("Enemy bị đánh! HP còn " + currentHP);
        if (currentHP <= 0)
        {
            currentState = Enemy3State.Die;
        }
    }
    void Die()
    {
        //Ngừng di chuyển
        slime3NavMeshAgent.isStopped = true;
        slime3NavMeshAgent.velocity = Vector3.zero;

        //âm thanh
        music.PlayOneShot(musicClip[4]);

        //chạy anim
        enemy3Animator.SetTrigger(Enemy_Constant.Enemy3DieHash);

        //Tắt profile
        miniProfilePanel.SetActive(false);

        if (!isDrop && lootTable != null && lootTable.Count > 0)
        {
            //lấy vị trí ngẫu nhiên trong danh sách lootTable
            int randomIndex = Random.Range(0, lootTable.Count);
            //random 1 trong các vật phẩm 
            LootItem3 randomLoot = lootTable[randomIndex];
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
    IEnumerator TakeDamageTextAppear(float amount)
    {
        takeDamageText.text = "- " + amount.ToString();
        yield return new WaitForSeconds(1f);
        takeDamageText.text = "";
    }
}
