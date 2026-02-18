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

    void Update()
    {
        ReadInput();
        ApplyGravity();
        Move();
        Rotate();
    }

    // ========================
    // ĐỌC INPUT
    // ========================
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

    // ========================
    // GRAVITY
    // ========================
    void ApplyGravity()
    {
        if (controller.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity += gravity * Time.deltaTime;
    }

    // ========================
    // DI CHUYỂN
    // ========================
    void Move()
    {
        Vector3 velocity = movement;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    // ========================
    // XOAY HƯỚNG
    // ========================
    void Rotate()
    {
        Vector3 lookDir = new Vector3(movement.x, 0, movement.z);

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}
