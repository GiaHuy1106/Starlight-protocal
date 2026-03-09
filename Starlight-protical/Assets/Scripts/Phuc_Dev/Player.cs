using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float gravity = -19.2f;
    [SerializeField] float jumpHeight = 3f;
    bool isGrounded;
    bool canMove;
    Vector2 Velocity;
    Vector2 moveInput;  
   

    private void Start()
    {
       
    }
    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }



        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    }

    private void FixedUpdate()
    {
        Movement();
        Velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(Velocity * Time.fixedDeltaTime);
    }


    void Movement()
    {
        if (!canMove) return;
        Vector3 Move = (transform.right * moveInput.x + transform.forward * moveInput.y) * Time.fixedDeltaTime * moveSpeed;
        controller.Move(Move);
    }


    public void LockMove(bool value)
    {
        canMove = value;
    }
}
