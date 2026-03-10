using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    public CharacterController controller;
    public PlayerInput playerInput;
    public float knockbackDamping = 6f;

    private Vector3 knockbackVelocity;
    private Vector3 movement;
    private float verticalVelocity;
    private float originalSpeed;
    private bool isSlowed;
    private void Start()
    {
        originalSpeed = moveSpeed;
    }
    void Update()
    {
        ReadInput();
        ApplyGravity();
        Move();
        Rotate();

        if(playerInput.attackInput)
        {
            TryAttack();
        }    
    }
   
    // ĐỌC INPUT  
    void ReadInput()
    {
        movement = new Vector3(
            playerInput.horizontalInput,
            0,
            playerInput.verticalInput
        );

        movement.Normalize();
        movement *= moveSpeed;
    }
   
    // GRAVITY    
    void ApplyGravity()
    {
        if (controller.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity += gravity * Time.deltaTime;
    }
  
    // DI CHUYỂN    
    void Move()
    {
        Vector3 velocity = movement;

        // cộng lực knockback
        velocity += knockbackVelocity;

        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        // giảm lực knockback theo thời gian
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
    }
   
    // XOAY HƯỚNG
    void Rotate()
    {
        Vector3 lookDir = new Vector3(movement.x, 0, movement.z);

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
    void TryAttack()
    {
        
        //Boss01 boss = FindAnyObjectByType<Boss01>();
        Boss01Health bossHealth = FindAnyObjectByType<Boss01Health>();
        if (bossHealth != null)
        {                  
            Debug.Log("Hit Boss!");
        }
        bossHealth.TakeDamage(25f, gameObject);
        Boss02Health boss02Health = FindAnyObjectByType<Boss02Health>();
        if(boss02Health != null)
        {
            Debug.Log("Hit Boss02!");
        }
        boss02Health.TakeDamage(25f, gameObject);
    }    
    // HÀM APPLY SLOW
    public void ApplySlow(float slowPercent, float duration)
    {
        if (!isSlowed)
        {
            StartCoroutine(SlowCoroutine(slowPercent, duration));
        }
    }

    IEnumerator SlowCoroutine(float slowPercent, float duration)
    {
        isSlowed = true;

        moveSpeed = originalSpeed * (1f - slowPercent);

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;

        isSlowed = false;
    }
    public void ApplyKnockBack(Vector3 force)
    {
        knockbackVelocity = force;
    }
}
