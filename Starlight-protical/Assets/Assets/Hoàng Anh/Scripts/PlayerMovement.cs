using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    public CharacterController controller;
    public PlayerInput playerInput;

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
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
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
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 3f))
        {
            Boss01 boss = hit.collider.GetComponentInParent<Boss01>();

            if (boss != null)
            {
                boss.TakeDamege(25f, gameObject);
                Debug.Log("Hit Boss!");
            }
        }
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
}
